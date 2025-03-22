using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCarController : MonoBehaviour
{
    public Transform aim;
    private void Start()
    {
        aim=GetComponent<Transform>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("åó¼ý"))
        {
            other.transform.position= aim.position;
        }
    }
}
