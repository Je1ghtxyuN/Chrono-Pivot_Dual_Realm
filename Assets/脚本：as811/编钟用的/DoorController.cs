using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public Transform door;
    
    public float speed = 5f;
    public Quaternion targetRotation;
    public void Open()
    {
        isOpen = true;
    }

    private void Update()
    {
        //debug”√
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Open();
        //}
        if (isOpen)
        {
            // Œª÷√≤Â÷µ
            door.rotation = Quaternion.Slerp(
                door.rotation,
                targetRotation,
                Time.deltaTime * speed
            );

        }
    }
}
   