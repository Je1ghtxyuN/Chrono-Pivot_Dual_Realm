using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas; // 引用你的UI菜单Canvas
    public Transform cameraTransform; // 引用玩家的摄像头
    public float distanceFromCamera = 2.0f; // 菜单与摄像头的距离

    private bool isMenuVisible = false;

    void Update()
    {
        // 检测菜单键是否被按下
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed) && menuButtonPressed)
        {
            ToggleMenu();
        }

        // 如果菜单可见，将其放置在摄像头前方
        if (isMenuVisible)
        {
            PositionMenuInFrontOfCamera();
        }
    }

    void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        menuCanvas.SetActive(isMenuVisible);
    }

    void PositionMenuInFrontOfCamera()
    {
        // 将菜单放置在摄像头前方
        menuCanvas.transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
        // 使菜单面向摄像头
        menuCanvas.transform.LookAt(cameraTransform);
        menuCanvas.transform.rotation = Quaternion.LookRotation(menuCanvas.transform.position - cameraTransform.position);
    }
}