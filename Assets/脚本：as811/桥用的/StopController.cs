using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class StopController : MonoBehaviour
{
    public Rigidbody arrow;
    private Transform aim;
    public Transform arrowsposition;
    private Quaternion initialRotation;
    private bool isIn;
    private void Start()
    {
        aim = GetComponent<Transform>();
        
    }
    public void Stop()
    {
        arrow.velocity=new Vector3(0f, 0f, 0f);
        initialRotation = arrowsposition.rotation;
        isIn = true;
    }
    private void Update()
    {
        if (isIn)
        {
            arrowsposition.position = aim.position;
            arrowsposition.rotation = initialRotation;
        }
    }
}
