using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFobidden : MonoBehaviour
{
    public GravityLogic GravityLogic;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GravityLogic.enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GravityLogic.enabled = true;
        }
    }
}
