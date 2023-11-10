using UnityEngine;

public class Thrust : MonoBehaviour
{
    public Buoyancy water;
    public float power;
    public string InputAxis;
    public Transform intake;
    public Transform outtake;
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (transform.TransformPoint(intake.localPosition).y <= water.GetHeight(transform.TransformPoint(intake.localPosition)))
        {
            if (transform.TransformPoint(outtake.localPosition).y <= water.GetHeight(transform.TransformPoint(outtake.localPosition)))
            {
                float mul = Input.GetAxis(InputAxis);
                Vector3 loc = (mul<0?intake:outtake).localPosition;
                Vector3 fwd = (mul < 0 ? intake : outtake).forward;
                Vector3 force = fwd * power * mul;
                Debug.DrawRay(transform.TransformPoint(loc), -force);
                rb.AddForceAtPosition(force,transform.TransformPoint(loc));
            }
        }
    }
    private void OnDrawGizmos()
    {
        bool inSub = transform.TransformPoint(intake.localPosition).y <= water.GetHeight(transform.TransformPoint(intake.localPosition));
        bool outSub = transform.TransformPoint(outtake.localPosition).y <= water.GetHeight(transform.TransformPoint(outtake.localPosition));
        Gizmos.color = Color.green * (inSub?1:0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(intake.localPosition),0.1f);
        Gizmos.color = Color.red * (outSub ? 1 : 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(outtake.localPosition), 0.1f);
        if (inSub && outSub)
        {
            Gizmos.color = Color.red;
        }
        else Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.TransformPoint(intake.localPosition), transform.TransformPoint(outtake.localPosition));

    }
}
