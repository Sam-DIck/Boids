using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public Buoyancy buoyancy;

    [Header("Appearance")]
    public Mesh boidMesh;
    public Material boidMaterial;

    ComputeBuffer posBuffer;
    ComputeBuffer velBuffer;
    ComputeBuffer accBuffer;


    [Header("BoidShader")]
    public ComputeShader Shader;

    public int boidCount;
    public float spawnRadius = 10;
    public float spawnSpeed = 1;


    int CalcForcesKernal;
    int UpdatePosVelKernal;
    uint threadGroupSize_calcForce;
    uint threadGroupSize_updatePos;
    Vector3[] outputPos;
    public Vector3[] outputVel;
    public Vector3[] outputAcc;

    public bool updatePos = true;

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
            boidPos.Add(Random.insideUnitSphere * spawnRadius);
            Vector2 horiPos = Random.insideUnitCircle * spawnSpeed;
            boidVel.Add(new Vector3(horiPos.x,0,horiPos.y));
        }
        posBuffer.SetData(boidPos.ToArray());
        velBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        velBuffer.SetData(boidVel.ToArray());
        accBuffer = new ComputeBuffer(boidCount, sizeof(float) * 3);
        //spherePosBuffer = new ComputeBuffer(spheres.Length, sizeof(float) * 3);
        //sphereRadBuffer = new ComputeBuffer(spheres.Length, sizeof(float));

        outputPos = new Vector3[boidCount];
        outputVel = new Vector3[boidCount];
        outputAcc = new Vector3[boidCount];

        posBuffer.GetData(outputPos);
        velBuffer.GetData(outputVel);
        accBuffer.GetData(outputAcc);
        Shader.SetBuffer(CalcForcesKernal, "posBuffer", posBuffer);
        Shader.SetBuffer(CalcForcesKernal, "velBuffer", velBuffer);
        Shader.SetBuffer(CalcForcesKernal, "accBuffer", accBuffer);
    }
    private void Update()
    {
        #region Calculate & Update Spheres
        //if (spherePosBuffer == null) Debug.Log(spherePosBuffer);
        //spherePosBuffer.SetData((from s in spheres select s.centre).ToArray());
        //sphereRadBuffer.SetData((from s in spheres select s.radius).ToArray());
        //Shader.SetBuffer(CalcForcesKernal, "sphereCentres", spherePosBuffer);
        //Shader.SetBuffer(CalcForcesKernal, "sphereRadius", sphereRadBuffer);
        #endregion
        #region Calculate Forces

        // Shader.SetBuffer(CalcForcesKernal, "posBuffer", posBuffer);
        // Shader.SetBuffer(CalcForcesKernal, "velBuffer", velBuffer);
        // Shader.SetBuffer(CalcForcesKernal, "accBuffer", accBuffer);
        Shader.SetFloat("deltaTime", Time.deltaTime);
        Shader.SetFloat("boids", boidCount);
        Shader.SetFloat("waterHeight", buoyancy.waterHeight);
        Shader.SetFloat("waveSize", buoyancy.waveSize);
        Shader.SetFloat("time", Time.time);
        int threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize_calcForce);
        Shader.Dispatch(CalcForcesKernal, threadGroups, 1, 1);
        #endregion
        #region Update Positions
        if (updatePos)
        {
            threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize_updatePos);
            Shader.SetBuffer(UpdatePosVelKernal, "posBuffer", posBuffer);
            Shader.SetBuffer(UpdatePosVelKernal, "velBuffer", velBuffer);
            Shader.SetBuffer(UpdatePosVelKernal, "accBuffer", accBuffer);
            Shader.Dispatch(UpdatePosVelKernal, threadGroups, 1, 1);
        }
        #endregion
        #region Retrieve Buffers
        posBuffer.GetData(outputPos);
        velBuffer.GetData(outputVel);
        accBuffer.GetData(outputAcc);
        #endregion
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
    


    void OnDisable()
    {
        posBuffer.Release();
        posBuffer = null;
        velBuffer.Release();
        velBuffer = null;
        accBuffer.Release();
        accBuffer = null;
        //spherePosBuffer.Release();
        //spherePosBuffer = null;
        //sphereRadBuffer.Release();
        //sphereRadBuffer = null;
    }
}

[System.Serializable]
public struct Sphere
{
    public Vector3 centre;
    public float radius;
}
