using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ContinuousRotator : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转轴（世界坐标系）")]
    public Vector3 rotationAxis = Vector3.up; // 默认绕Y轴旋转

    [Tooltip("旋转速度（度/秒）")]
    public float degreesPerSecond = 30f;

    [Tooltip("是否使用本地坐标系")]
    public bool useLocalSpace = false;


    void Start()
    {


        // 自动归一化旋转轴
        rotationAxis = rotationAxis.normalized;
    }

    void Update()
    {
        PerformRotation();
    }

    void PerformRotation()
    {
        // 计算本帧旋转量
        float rotationAmount = degreesPerSecond * Time.deltaTime;

        // 根据坐标系选择旋转空间
        Space space = useLocalSpace ? Space.Self : Space.World;

        // 执行旋转
        transform.Rotate(rotationAxis, rotationAmount, space);
    }

    
}