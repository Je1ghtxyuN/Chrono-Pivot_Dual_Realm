using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltReceiver : MonoBehaviour
{
    public Transform salt;
    public Transform locker;
    public Transform endposition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == salt.transform)
        {
            other.gameObject.SetActive(false);
            locker.position=endposition.position;
        }
    }
}
