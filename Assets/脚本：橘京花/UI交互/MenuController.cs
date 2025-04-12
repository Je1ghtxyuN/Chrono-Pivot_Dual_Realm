using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public List<GameObject> menuCanvases; // 引用多个UI菜单Canvas

    private bool isMenuVisible = false;
    private InputDevice leftHandDevice;
    private bool lastLeftMenuButtonState = false; // 跟踪上一帧的按钮状态
    private bool wasEscapeKeyPressed = false; // 跟踪上一帧的ESC键状态

    void Start()
    {
        SetAllCanvasesActive(false); // 初始化隐藏菜单
        InitializeLeftHandDevice();  // 初始化左手设备
    }

    void Update()
    {
        // 如果设备无效，尝试重新初始化
        if (!leftHandDevice.isValid)
        {
            InitializeLeftHandDevice();
        }

        // 检测左手菜单键状态变化
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool currentMenuButtonState))
        {
            // 只有当按钮状态改变时才处理
            if (currentMenuButtonState != lastLeftMenuButtonState)
            {
                // 只在按钮按下时触发（而不是释放时）
                if (currentMenuButtonState)
                {
                    ToggleMenu();
                }
                lastLeftMenuButtonState = currentMenuButtonState;
            }
        }

        // 检测键盘ESC键状态变化（用于调试）
        bool isEscapeKeyPressed = Input.GetKey(KeyCode.Escape);
        if (isEscapeKeyPressed && !wasEscapeKeyPressed)
        {
            ToggleMenu();
        }
        wasEscapeKeyPressed = isEscapeKeyPressed;
    }

    private void InitializeLeftHandDevice()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count > 0)
        {
            leftHandDevice = leftHandDevices[0];
            Debug.Log("Left hand device initialized: " + leftHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Left hand device not found!");
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