using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Thrust : MonoBehaviour
{
    public Buoyancy water;
    public float power;
    public string InputAxis;
    public Vector3 intake;
    public Vector3 outtake;
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (transform.TransformPoint(intake).y <= water.GetHeight(transform.TransformPoint(intake)))
        {
            if (transform.TransformPoint(outtake).y <= water.GetHeight(transform.TransformPoint(outtake)))
            {
                float mul = Input.GetAxis(InputAxis);
                Vector3 loc = mul<0?intake:outtake;
                Vector3 force = transform.forward * power * mul;
                Debug.DrawRay(transform.TransformPoint(loc), -force);
                rb.AddForceAtPosition(force,transform.TransformPoint(loc));
            }
        }
    }
    private void OnDrawGizmos()
    {
        bool inSub = transform.TransformPoint(intake).y <= water.GetHeight(transform.TransformPoint(intake));
        bool outSub = transform.TransformPoint(outtake).y <= water.GetHeight(transform.TransformPoint(outtake));
        Gizmos.color = Color.green * (inSub?1:0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(intake),0.1f);
        Gizmos.color = Color.red * (outSub ? 1 : 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(outtake), 0.1f);
        if (inSub && outSub)
        {
            Gizmos.color = Color.red;
        }
        else Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.TransformPoint(intake), transform.TransformPoint(outtake));

    }
}
