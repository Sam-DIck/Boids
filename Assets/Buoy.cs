using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Score.score += 1;
    }
}
