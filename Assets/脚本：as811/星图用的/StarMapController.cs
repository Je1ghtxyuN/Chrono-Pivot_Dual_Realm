using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class StarMapController : MonoBehaviour
{
    private ShotController shotController;
    
    public XRRayInteractor rightRayInteractor; // 右手柄射线组件
    public Transform arrow;
    private Transform aim;
    private ArrowCarController arrowCar;
    private void Start()
    {
        arrowCar=FindObjectOfType<ArrowCarController>();
        aim=arrowCar.GetComponent<Transform>();
        shotController=FindObjectOfType<ShotController>();
        //rightRayInteractor = GetRayInteractor(XRNode.RightHand);
        if (rightRayInteractor == null)
        {
            Debug.LogError("Right XRRayInteractor not found in the scene!");
        }
    }
    private void Update()
    {
        // 检查是否被右手柄射线瞄准
        bool isHighlighted = IsHighlighted();

        // 检查右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 如果被右手柄射线瞄准且按下扳机键，调用 Clock() 函数
        if (isHighlighted && isRightTriggerPressed&&arrow.position==aim.position)
        {
            shotController.ArrowShot();
        }
    }

    // 检查是否被右手柄射线瞄准
    private bool IsHighlighted()
    {
        // 检查右手柄射线是否瞄准当前物体
        if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit rightHit))
        {
            if (rightHit.collider != null && rightHit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    // 检查右手柄的扳机键是否按下
    private bool IsTriggerPressed(XRNode handNode)
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(handNode, devices);
        if (devices.Count > 0)
        {
            if (devices[0].TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
            {
                return triggerValue;
            }
        }
        return false;
    }

    // 获取指定手柄的射线组件
    private XRRayInteractor GetRayInteractor(XRNode handNode)
    {
        XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();
        foreach (XRRayInteractor rayInteractor in rayInteractors)
        {
            if (rayInteractor.gameObject.name.Contains(handNode == XRNode.RightHand ? "Right" : "Left"))
            {
                return rayInteractor;
            }
        }
        return null;
    }
}
