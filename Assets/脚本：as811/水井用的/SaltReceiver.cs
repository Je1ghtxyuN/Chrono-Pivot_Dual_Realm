using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltReceiver : MonoBehaviour
{
    public Transform salt;
    public Transform locker;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == salt.transform)
        {
            other.gameObject.SetActive(false);
            locker.gameObject.SetActive(true);
        }
    }
}
