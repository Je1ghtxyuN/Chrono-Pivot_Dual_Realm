using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class SceneSwitchOnSelect : MonoBehaviour
{
    public Transform player; // 玩家的位置（通常是 XR Origin 或 Camera）
    public float interactionDistance = 2.0f; // 交互的最大距离
    public string targetSceneName = "01"; // 要切换的目标场景名称

    private InputDevice rightHandDevice; // 右手柄设备
    private InputDevice leftHandDevice;  // 左手柄设备
    private XRRayInteractor rightRayInteractor; // 右手柄射线组件
    private XRRayInteractor leftRayInteractor;  // 左手柄射线组件
    private bool isRightRayHittingObject = false; // 右手射线是否射中物体
    private bool isLeftRayHittingObject = false;  // 左手射线是否射中物体

    void Start()
    {
        // 初始化左右手柄设备
        InitializeRightHandDevice();
        InitializeLeftHandDevice();

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

    void Update()
    {
        // 检查设备是否有效，如果无效则重新初始化
        if (!rightHandDevice.isValid)
        {
            InitializeRightHandDevice();
        }
        if (!leftHandDevice.isValid)
        {
            InitializeLeftHandDevice();
        }

        // 检查左右手柄的射线是否射中物体
        CheckRayHit(XRNode.RightHand, ref isRightRayHittingObject);
        CheckRayHit(XRNode.LeftHand, ref isLeftRayHittingObject);

        // 检查玩家与物体的距离
        if (IsPlayerNearObject(transform))
        {
            // 检查右手射线是否射中物体，并且是否按下了右手柄的扳机键
            if (isRightRayHittingObject && IsRightTriggerPressed())
            {
                Debug.Log("Right trigger pressed and object hit. Switching scene...");
                // 切换场景
                SceneManager.LoadScene(targetSceneName);
            }

            // 检查左手射线是否射中物体，并且是否按下了左手柄的扳机键
            if (isLeftRayHittingObject && IsLeftTriggerPressed())
            {
                Debug.Log("Left trigger pressed and object hit. Switching scene...");
                // 切换场景
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    // 判断玩家是否在物体附近
    private bool IsPlayerNearObject(Transform objectTransform)
    {
        float distance = Vector3.Distance(player.position, objectTransform.position);
        return distance <= interactionDistance;
    }

    // 初始化右手柄设备
    private void InitializeRightHandDevice()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
        {
            rightHandDevice = devices[0];
            Debug.Log("Right hand device initialized: " + rightHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Right hand device not found!");
        }
    }

    // 初始化左手柄设备
    private void InitializeLeftHandDevice()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0)
        {
            leftHandDevice = devices[0];
            Debug.Log("Left hand device initialized: " + leftHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Left hand device not found!");
        }
    }

    // 检查右手柄的扳机键是否按下
    private bool IsRightTriggerPressed()
    {
        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
            {
                Debug.Log("Right Trigger Value: " + triggerValue);
                return triggerValue;
            }
        }
        else
        {
            Debug.LogWarning("Right hand device is not valid!");
        }
        return false;
    }

    // 检查左手柄的扳机键是否按下
    private bool IsLeftTriggerPressed()
    {
        if (leftHandDevice.isValid)
        {
            if (leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
            {
                Debug.Log("Left Trigger Value: " + triggerValue);
                return triggerValue;
            }
        }
        else
        {
            Debug.LogWarning("Left hand device is not valid!");
        }
        return false;
    }

    // 检查射线是否射中物体
    private void CheckRayHit(XRNode handNode, ref bool isRayHittingObject)
    {
        XRRayInteractor rayInteractor = handNode == XRNode.RightHand ? rightRayInteractor : leftRayInteractor;

        if (rayInteractor != null)
        {
            // 获取射线击中的物体
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // 检查击中的物体是否是当前物体
                if (hit.transform == transform)
                {
                    isRayHittingObject = true;
                    Debug.Log((handNode == XRNode.RightHand ? "Right" : "Left") + " ray hit object: " + hit.transform.name);
                    return;
                }
            }
        }
        isRayHittingObject = false;
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