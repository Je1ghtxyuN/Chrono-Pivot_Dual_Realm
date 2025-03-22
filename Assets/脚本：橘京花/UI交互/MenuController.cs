using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public List<GameObject> menuCanvases; // 引用多个UI菜单Canvas
    public Transform cameraTransform; // 引用玩家的摄像头
    public float distanceFromCamera = 2.0f; // 菜单与摄像头的距离

    private bool isMenuVisible = false;

    void Start()
    {
        // 初始化时将所有Canvas设置为非激活状态
        SetAllCanvasesActive(false);
    }

    void Update()
    {
        // 检测菜单键是否被按下（单击）
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed) && menuButtonPressed)
        {
            Debug.LogWarning("检测到菜单键被按下。");
            ToggleMenu();
        }

        // 如果菜单可见，将所有Canvas放置在摄像头前方
        if (isMenuVisible)
        {
            PositionAllCanvasesInFrontOfCamera();
        }
    }

    void ToggleMenu()
    {
        // 切换菜单状态
        isMenuVisible = !isMenuVisible;

        // 设置所有Canvas的激活状态
        SetAllCanvasesActive(isMenuVisible);

        // 输出菜单状态
        if (isMenuVisible)
        {
            Debug.LogWarning("所有菜单已显示。");
        }
        else
        {
            Debug.LogWarning("所有菜单已隐藏。");
        }
    }

    void SetAllCanvasesActive(bool isActive)
    {
        // 设置所有Canvas的激活状态
        foreach (var canvas in menuCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(isActive);
            }
        }
    }

    void PositionAllCanvasesInFrontOfCamera()
    {
        // 将所有Canvas放置在摄像头前方
        foreach (var canvas in menuCanvases)
        {
            if (canvas != null)
            {
                canvas.transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
                canvas.transform.LookAt(cameraTransform);
                canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraTransform.position);
            }
        }

        Debug.LogWarning("所有菜单已放置在摄像头前方。");
    }
}