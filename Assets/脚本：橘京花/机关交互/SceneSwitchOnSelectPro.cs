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
    private XRRayInteractor rayInteractor; // 手柄射线组件
    private bool isRayHittingObject = false; // 射线是否射中物体

    void Start()
    {
        // 初始化右手柄设备
        InitializeRightHandDevice();

        // 获取右手柄的射线组件
        rayInteractor = FindObjectOfType<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("XRRayInteractor not found in the scene!");
        }
    }

    void Update()
    {
        // 检查设备是否有效，如果无效则重新初始化
        if (!rightHandDevice.isValid)
        {
            InitializeRightHandDevice();
        }

        // 检查射线是否射中物体
        CheckRayHit();

        // 检查玩家与物体的距离
        if (IsPlayerNearObject(transform))
        {
            // 检查射线是否射中物体，并且是否按下了右手柄的扳机键
            if (isRayHittingObject && IsRightTriggerPressed())
            {
                Debug.Log("Trigger pressed and object hit. Switching scene...");
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

    // 检查右手柄的扳机键是否按下
    private bool IsRightTriggerPressed()
    {
        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
            {
                Debug.Log("Trigger Value: " + triggerValue);
                return triggerValue;
            }
        }
        return false;
    }

    // 检查射线是否射中物体
    private void CheckRayHit()
    {
        if (rayInteractor != null)
        {
            // 获取射线击中的物体
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // 检查击中的物体是否是当前物体
                if (hit.transform == transform)
                {
                    isRayHittingObject = true;
                    Debug.Log("Ray hit object: " + hit.transform.name);
                    return;
                }
            }
        }
        isRayHittingObject = false;
    }
}