using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CentreOfMass : MonoBehaviour
{
    public Vector3 centerOfMass;
    void Update()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.TransformPoint(centerOfMass), 0.1f);
        Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh,
            transform.position,
            transform.rotation,
            transform.lossyScale);
    }
}
