using UnityEngine;

public class Steer : MonoBehaviour
{
    public float angle;
    public string SteeringAxis;

    void Update()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, angle * Input.GetAxis(SteeringAxis), 0));
    }
}
