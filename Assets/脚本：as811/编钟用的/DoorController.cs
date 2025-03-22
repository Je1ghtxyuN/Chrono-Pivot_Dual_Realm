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
    public Transform target;
    public float speed = 5f;

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
            door.position = Vector3.Lerp(
            door.position,
                target.position,
                Time.deltaTime * speed
            );

        }
    }
}
   