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
    public ShotController st;
    private void Start()
    {
        aim = GetComponent<Transform>();
        st=GetComponent<ShotController>();
    }
    public void Stop()
    {
        arrow.velocity=new Vector3(0f, 0f, 0f);
        initialRotation = arrowsposition.rotation;
        isIn = true;
        st.isflying = false;
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
