using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public List<GameObject> menuCanvases; // 引用多个UI菜单Canvas

    private bool isMenuVisible = false;
    private InputDevice rightHandDevice;
    private bool wasMenuButtonPressed = false; // 跟踪上一帧的按钮状态
    private bool wasEscapeKeyPressed = false; // 跟踪上一帧的ESC键状态

    void Start()
    {
        SetAllCanvasesActive(false); // 初始化隐藏菜单
        InitializeRightHandDevice();  // 初始化右手设备
    }

    void Update()
    {
        // 如果设备无效，尝试重新初始化
        if (!rightHandDevice.isValid)
        {
            InitializeRightHandDevice();
        }

        // 检测菜单键状态变化
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuButtonPressed))
        {
            // 只有当按钮从"按下"变为"未按下"时才切换菜单
            if (isMenuButtonPressed && !wasMenuButtonPressed)
            {
                ToggleMenu();
            }
            wasMenuButtonPressed = isMenuButtonPressed;
        }

        // 检测键盘ESC键状态变化（用于调试）
        bool isEscapeKeyPressed = Input.GetKey(KeyCode.Escape);
        if (isEscapeKeyPressed && !wasEscapeKeyPressed)
        {
            ToggleMenu();
        }
        wasEscapeKeyPressed = isEscapeKeyPressed;
    }

    private void InitializeRightHandDevice()
    {
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightHandDevice = rightHandDevices[0];
            Debug.Log("Right hand device initialized: " + rightHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Right hand device not found!");
        }
    }

    void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        SetAllCanvasesActive(isMenuVisible);
        Debug.Log(isMenuVisible ? "菜单已显示" : "菜单已隐藏");
    }

    void SetAllCanvasesActive(bool isActive)
    {
        foreach (var canvas in menuCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(isActive);
            }
        }
    }
}