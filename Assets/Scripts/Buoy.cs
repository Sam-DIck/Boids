using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buoy : MonoBehaviour
{
    public float rate = 0;
    [SerializeField] Text bonusText;
    public string playerTag;
    public float stay;
    GameObject stayObject;
    private void OnTriggerEnter(Collider other)
    {
        if (stayObject != null) return;
        if (other.gameObject.CompareTag(playerTag))
        {
            stayObject = other.gameObject;
            stay = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {


        if (other.gameObject.CompareTag(playerTag))
        {
            if (other.attachedRigidbody.velocity.magnitude < 1)
            {
                Score.score += (int)(stay * rate);
                stay = 0;
                stayObject = null;
            }
            else if (stayObject == null)
            {
                stayObject = other.gameObject;
                stay = 0;
            }
            else
            { 
                stay += other.attachedRigidbody.velocity.magnitude * Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            Score.score += (int)(stay * rate);
            stay = 0;
            stayObject = null;
        }
    }

    private void Update()
    {
        bonusText.text = $"+{(int)(stay * rate)}";
        bonusText.enabled = (stayObject != null);
    }



}
