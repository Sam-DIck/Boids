using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Buoyancy : MonoBehaviour
{
    [Tooltip("Density(kg/m^3)")]
    public float FluidDensity = 1000;
    public Vector3 gravity;
    public float waterHeight;
    public float waveSize;
    public float waveLength;
    public float waveSpeed;
    public float turbulance;
    


    List<(Vector3 origin, Vector3 direction)> forces = new List<(Vector3 origin, Vector3 direction)>();
    void FixedUpdate()
    {
        forces.Clear();
        foreach (Rigidbody rb in FindObjectsOfType(typeof(Rigidbody)))
        {
            rb.useGravity = false;

            Vector3 gravForce = gravity * rb.mass * 2;
            rb.AddForce(gravForce);


            if (rb.GetComponent<MeshCollider>() != null)
            {
                Mesh mesh = rb.GetComponent<MeshCollider>().sharedMesh;
                Vector3[] verts = (from vert in mesh.vertices select rb.transform.TransformPoint(vert) - transform.position).ToArray();
                Vector3 mid = Vector3.zero;
                foreach (Vector3 v in verts)
                {
                    mid += v / verts.Length;
                }
                (Vector3 pos, bool moved)[] movedVerts = (from v in verts select (clampHeight(v, out bool m), m)).ToArray();
                (Vector3 pos, bool moved) movedMid = (clampHeight(mid, out bool m), m);
                int[] tris = mesh.triangles;
                for (int i = 0; i < tris.Length; i += 3)
                {
                    Vector3 a = movedVerts[tris[i]].pos;
                    Vector3 b = movedVerts[tris[i + 1]].pos;
                    Vector3 c = movedVerts[tris[i + 2]].pos;
                    Vector3 d = movedMid.pos;
                    if (!(movedVerts[tris[i]].moved
                        && movedVerts[tris[i + 1]].moved
                        && movedVerts[tris[i] + 2].moved
                        && movedMid.moved))
                    {
                        Vector3 forceMid = (a + b + c + d) / 4;

                        float volume = SignedVolumeOfTetrahedron(a, b, c, d);
                        Vector3 buoyancyForce = gravity * volume * FluidDensity*2;
                        Vector3 force = buoyancyForce + Random.insideUnitSphere * turbulance, position = forceMid + transform.position;
                        //Debug.DrawRay(position, force);
                        rb.AddForceAtPosition(force, position);
                    }
                }
            }
        }
    }

    private static float SignedVolumeOfTetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ad = d - a;
        Vector3 cp = Vector3.Cross(ab, ac);
        float volume = Vector3.Dot(cp, ad) / 6;
        return volume;
    }
    Vector3 clampHeight(Vector3 v, out bool moved)
    {
        v = new Vector3(v.x, v.y, v.z);
        moved = clampHeight(ref v);
        return v;
    }
    bool clampHeight(ref Vector3 v)
    {
        float height = GetHeight(v);
        if (v.y > height) { v = new Vector3(v.x,height,v.z); return true; }
        return false;
    }
    public float GetHeight(Vector3 pos)
    {
        float t = Time.time;
        return 1 - Mathf.Exp(Mathf.Sin((pos.x + t*waveSpeed)*waveLength) - 1) * waveSize;
    }

}
