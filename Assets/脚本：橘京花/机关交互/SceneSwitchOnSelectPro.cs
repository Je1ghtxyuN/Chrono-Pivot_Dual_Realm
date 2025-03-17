using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using UnityEngine.XR;

public class SceneSwitchOnSelectPro : MonoBehaviour
{
    public Transform player; // 玩家的位置（通常是 XR Origin 或 Camera）
    public TimetravelController timetravelController; // 直接拖入 TimetravelController

    private XRRayInteractor rightRayInteractor; // 右手柄射线组件
    private XRRayInteractor leftRayInteractor;  // 左手柄射线组件

    private XRBaseInteractable interactable; // 当前物体的交互组件

    private bool isPositionSwitchBlocked = false; // 是否禁止切换位置
    private float positionSwitchBlockTimer = 0f; // 禁止切换位置的计时器
    private const float positionSwitchBlockDuration = 2f; // 禁止切换位置的时间（2秒）

    void Start()
    {
        // 获取当前物体的 XRBaseInteractable 组件
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("XRBaseInteractable component not found on this object!");
        }

        // 获取左右手柄的射线组件
        rightRayInteractor = GetRayInteractor(XRNode.RightHand);
        leftRayInteractor = GetRayInteractor(XRNode.LeftHand);

        if (rightRayInteractor == null)
        {
            Debug.LogError("Right XRRayInteractor not found in the scene!");
        }
        if (leftRayInteractor == null)
        {
            Debug.LogError("Left XRRayInteractor not found in the scene!");
        }

        // 检查 TimetravelController 是否已赋值
        if (timetravelController == null)
        {
            Debug.LogError("TimetravelController is not assigned! Please drag and drop the TimetravelController component.");
        }
    }

    void Update()
    {
        // 如果禁止切换位置，更新计时器
        if (isPositionSwitchBlocked)
        {
            positionSwitchBlockTimer -= Time.deltaTime;
            if (positionSwitchBlockTimer <= 0f)
            {
                isPositionSwitchBlocked = false; // 计时器结束，允许切换位置
                Debug.LogWarning("禁止切换位置已解除，可以再次切换位置。");
            }
        }

        // 检查物体是否被高亮（即是否被射线选中）
        bool isHighlighted = interactable.isHovered;

        // 如果物体被高亮，输出调试信息
        if (isHighlighted)
        {
            Debug.LogWarning("物体被高亮，射线选中。");
        }

        // 检查右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 如果右手柄扳机键按下，输出调试信息
        if (isRightTriggerPressed)
        {
            Debug.LogWarning("右手柄扳机键按下。");
        }

        // 检查左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);

        // 如果左手柄扳机键按下，输出调试信息
        if (isLeftTriggerPressed)
        {
            Debug.LogWarning("左手柄扳机键按下。");
        }

        // 如果满足所有条件且未禁止切换位置，调用 Timetravel 方法
        if (isHighlighted && (isRightTriggerPressed || isLeftTriggerPressed) && !isPositionSwitchBlocked)
        {
            Debug.LogWarning("条件满足，正在切换玩家位置...");

            // 调用 Timetravel 方法
            timetravelController.Timetravel();

            // 切换位置后禁止再次切换位置
            isPositionSwitchBlocked = true;
            positionSwitchBlockTimer = positionSwitchBlockDuration;
            Debug.LogWarning("切换位置成功，2 秒内禁止再次切换位置。");
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