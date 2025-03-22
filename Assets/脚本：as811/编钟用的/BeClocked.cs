using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BeClocked : MonoBehaviour
{
    public bool isClocked = false; // 是否被敲钟
    private ClockController clockController; // 引用 ClockController
    private BeClocked beClocked; // 当前物体的 BeClocked 组件

    private XRRayInteractor rightRayInteractor; // 右手柄射线组件
    private XRRayInteractor leftRayInteractor;  // 左手柄射线组件

    private void Start()
    {
        // 获取 ClockController
        clockController = FindObjectOfType<ClockController>();
        if (clockController == null)
        {
            Debug.LogError("ClockController not found in the scene!");
        }

        // 获取当前物体的 BeClocked 组件
        beClocked = GetComponent<BeClocked>();
        if (beClocked == null)
        {
            Debug.LogError("BeClocked component not found on this object!");
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
    }

    private void Update()
    {
        // 检查是否被射线瞄准
        bool isHighlighted = IsHighlighted();

        // 检查右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 检查左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);

        // 如果被射线瞄准且按下扳机键，调用 Clock() 函数
        if (isHighlighted && (isRightTriggerPressed || isLeftTriggerPressed))
        {
            Clock();
        }
    }

    // 检查是否被射线瞄准
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

        // 检查左手柄射线是否瞄准当前物体
        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit leftHit))
        {
            if (leftHit.collider != null && leftHit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
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

    // 敲钟逻辑
    public void Clock()
    {
        Debug.Log("碰到了");
        if (clockController.nowClockedNum < clockController.maxNum)
        {
            if (beClocked == clockController.beClockeds[clockController.nowClockedNum])
            {
                isClocked = true;
                FindObjectOfType<ClockController>().PlayOnce();
                Debug.Log("顺序正确");
            }
            // 顺序不对，重置
            else
            {
                isClocked = false;
                clockController.nowClockedNum = 0;
                clockController.nowNum = 0;
                Debug.Log("重置");
            }
        }
        else
        {
            Debug.Log("已完成，请勿重复碰钟表");
        }
    }
}