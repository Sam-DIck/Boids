using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidsManagerShader : MonoBehaviour
{
    public Mesh boidMesh;
    public Material boidMaterial;

    ComputeBuffer posBuffer;
    ComputeBuffer velBuffer;
    ComputeBuffer accBuffer;
    ComputeBuffer treeBuffer;
    public ComputeShader Shader;

    public int boidCount;

    int CalcForcesKernal;
    int UpdatePosVelKernal;
    uint threadGroupSize_calcForce;
    uint threadGroupSize_updatePos;
    Vector3[] outputPos;
    public Vector3[] outputVel;
    public Vector3[] outputAcc;

    void OnEnable()
    {
        CalcForcesKernal = Shader.FindKernel("UpdateBoidForces");
        UpdatePosVelKernal = Shader.FindKernel("UpdateBoidPos");
        Shader.GetKernelThreadGroupSizes(CalcForcesKernal, out threadGroupSize_calcForce, out _, out _);
        Shader.GetKernelThreadGroupSizes(UpdatePosVelKernal, out threadGroupSize_updatePos, out _, out _);

        posBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        List<Vector3> boidPos = new List<Vector3>();
        List<Vector3> boidVel = new List<Vector3>();
        for (int i = 0; i < boidCount; i++)
        {
            boidPos.Add(Random.insideUnitSphere*10);
            boidVel.Add(Random.insideUnitSphere);
        }
        posBuffer.SetData(boidPos.ToArray());
        velBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        velBuffer.SetData(boidVel.ToArray());
        accBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        treeBuffer = new ComputeBuffer(boidCount, sizeof(int));
        outputPos = new Vector3[boidCount];
        outputVel = new Vector3[boidCount];
        outputAcc = new Vector3[boidCount];
        posBuffer.GetData(outputPos);
        velBuffer.GetData(outputVel);
        accBuffer.GetData(outputAcc);
    }
    private void Update()
    {



        #region Calculate Forces

        //Shader.SetBuffer(CalcForcesKernal, "treeBuffer", treeBuffer);
        Shader.SetBuffer(CalcForcesKernal, "posBuffer", posBuffer);
        Shader.SetBuffer(CalcForcesKernal, "velBuffer", velBuffer);
        Shader.SetBuffer(CalcForcesKernal, "accBuffer", accBuffer);
        Shader.SetBuffer(CalcForcesKernal, "treeBuffer", treeBuffer);
        Shader.SetFloat("deltaTime", Time.deltaTime);
        Shader.SetFloat("boids", boidCount);
        int threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize_calcForce);
        Shader.Dispatch(CalcForcesKernal, threadGroups, 1, 1);
        #endregion
        #region Update Positions
        threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize_updatePos);
        Shader.SetBuffer(UpdatePosVelKernal, "posBuffer", posBuffer);
        Shader.SetBuffer(UpdatePosVelKernal, "velBuffer", velBuffer);
        Shader.SetBuffer(UpdatePosVelKernal, "accBuffer", accBuffer);
        Shader.Dispatch(UpdatePosVelKernal, threadGroups, 1, 1);
        #endregion

        posBuffer.GetData(outputPos);
        velBuffer.GetData(outputVel);
        accBuffer.GetData(outputAcc);


        #region Draw Boids
        
        if (outputPos != null)
        {
            List<Matrix4x4> boidMats = new List<Matrix4x4>();
            for (int i = 0; i < outputPos.Count(); i++)
            {
                Vector3 up = Vector3.Cross(outputVel[i], outputAcc[i]);
                boidMats.Add(Matrix4x4.TRS(outputPos[i], Quaternion.LookRotation(outputVel[i],up), transform.localScale));
            }
            Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, boidMats.ToArray());
        }
        
        #endregion
    }

    private void OnDrawGizmos()
    {
        if (outputPos != null&&outputPos.Length>0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(outputPos[0], outputVel[0]);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(outputPos[0], outputAcc[0]);
        }
    }

    void OnDisable()
    {
        posBuffer.Release();
        posBuffer = null;
        velBuffer.Release();
        velBuffer = null;
        accBuffer.Release();
        accBuffer = null;
        treeBuffer.Release();
        treeBuffer = null;
    }
}
