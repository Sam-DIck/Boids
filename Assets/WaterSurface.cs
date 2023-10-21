using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    public Buoyancy buoyancy;
    private void Update()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Vector3[] verts = mf.mesh.vertices;
        verts = (from vert in verts select new Vector3(vert.x,
                                                        buoyancy.GetHeight(transform.TransformPoint(vert)),
                                                        vert.z)
                 ).ToArray();
        mf.mesh.SetVertices(verts);
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
    }
}
