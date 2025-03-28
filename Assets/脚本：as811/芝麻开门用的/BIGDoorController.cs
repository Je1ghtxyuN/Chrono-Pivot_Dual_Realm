using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIGDoorController : MonoBehaviour
{
    [Header("钥匙，门，钥匙孔，钥匙附着目标选择")]
    public Transform key;
    public Transform keyHole;
    public Transform door1, door2;
    public Transform targetPosition;
    [Header("钥匙移动速度")]
    public float moveSpeed = 5f;    // 移动平滑度
    [Header("门移动速度")]
    public float rotateSpeed = 10f; // 旋转平滑度

    private bool isTriggered = false;
    private bool isDoorOpen = false;
    private Vector3 initialPosition;
    //private Quaternion initialRotation;
    //[Header("钥匙旋转")]
    //public Quaternion targetRot = Quaternion.LookRotation(-Vector3.forward);
    [Header("门旋转")]
    public Quaternion targetRotDoor1 = Quaternion.LookRotation(-Vector3.up);
    public Quaternion targetRotDoor2 = Quaternion.LookRotation(-Vector3.up);
    void OnTriggerEnter(Collider other)
    {
        if (other.transform == key)
        {
            isTriggered = true;
            initialPosition = key.position;
            //initialRotation = key.rotation;
            Debug.Log("钥匙插进去了");
        }
    }
    
    void Update()
    {
        if (isTriggered)
        {
            // 位置插值
            key.position = Vector3.Lerp(
                key.position,
                targetPosition.position,
                Time.deltaTime * moveSpeed
            );

            //// 旋转插值

            //key.rotation = Quaternion.Slerp(
            //    key.rotation,
            //    targetRot,
            //    Time.deltaTime * rotateSpeed
            //);
            //keyHole.rotation = key.rotation;
            if (key.position == targetPosition.position) isDoorOpen = true;
        }
        
        if (isDoorOpen)
        {
            door1.rotation = Quaternion.Slerp(
              door1.rotation,
              targetRotDoor1,
              Time.deltaTime * rotateSpeed
          );
            door2.rotation = Quaternion.Slerp(
               door2.rotation,
               targetRotDoor2,
               Time.deltaTime * rotateSpeed
           );
        }

    }
   

}
