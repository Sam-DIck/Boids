using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float spinRate;
    void Update()
    {
        transform.Rotate(Vector3.up, spinRate*Time.deltaTime);
    }
}
