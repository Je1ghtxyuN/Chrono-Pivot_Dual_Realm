using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class StarMapController : MonoBehaviour
{
    public ShotController shotController;
    public XRRayInteractor rightRayInteractor; // 右手柄射线组件
    public ArrowCarController ac;
    public ArrowCarController arrowCar;

    private XRBaseInteractable interactable; // 当前物体的交互组件

    private bool isInteractionBlocked = false; // 是否禁止交互
    private float interactionBlockTimer = 0f; // 禁止交互的计时器
    private const float interactionBlockDuration = 0.5f; // 禁止交互的时间（0.5秒）

    private void Start()
    {
        // 获取当前物体的 XRBaseInteractable 组件
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("XRBaseInteractable component not found on this object!");
        }

        if (rightRayInteractor == null)
        {
            Debug.LogError("Right XRRayInteractor not found in the scene!");
        }
    }

    private void Update()
    {
        // 如果禁止交互，更新计时器
        if (isInteractionBlocked)
        {
            interactionBlockTimer -= Time.deltaTime;
            if (interactionBlockTimer <= 0f)
            {
                isInteractionBlocked = false; // 计时器结束，允许交互
                Debug.LogWarning("禁止交互已解除，可以再次交互。");
            }
        }

        // 检查物体是否被高亮（即是否被射线选中）
        bool isHighlighted = interactable.isHovered;

        // 检查右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 如果满足所有条件且未禁止交互，调用 ArrowShot 方法
        if (isHighlighted && isRightTriggerPressed && ac.isIn && !isInteractionBlocked)
        {
            Debug.LogWarning("条件满足，正在发射箭头...");

            // 调用射击方法
            shotController.ArrowShot();

            // 交互后禁止再次交互
            isInteractionBlocked = true;
            interactionBlockTimer = interactionBlockDuration;
            Debug.LogWarning("发射成功，0.5秒内禁止再次发射。");
        }
    }

    // 检查手柄的扳机键是否按下
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
}