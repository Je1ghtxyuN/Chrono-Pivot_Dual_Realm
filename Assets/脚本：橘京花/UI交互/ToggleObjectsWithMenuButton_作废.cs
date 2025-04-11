using UnityEngine;
using UnityEngine.XR;

public class ToggleObjectsWithMenuButton_作废 : MonoBehaviour
{
    public GameObject objectToActivate; // 需要激活的物体
    public GameObject objectToDeactivate; // 需要停止激活的物体

    private bool isObjectActive = false; // 当前激活状态
    private bool isMenuButtonPressed = false; // 记录菜单键按下状态

    void Start()
    {
        // 初始化时设置物体的初始状态
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(isObjectActive);
        }
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(!isObjectActive);
        }
    }

    void Update()
    {
        // 检测左手柄的菜单键是否被按下（单击）
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed))
        {
            if (menuButtonPressed && !isMenuButtonPressed) // 只在按下瞬间触发
            {
                Debug.LogWarning("检测到左手柄菜单键被按下。");
                ToggleObjects();
                isMenuButtonPressed = true;
            }
            else if (!menuButtonPressed)
            {
                isMenuButtonPressed = false;
            }
        }
    }

    void ToggleObjects()
    {
        // 切换激活状态
        isObjectActive = !isObjectActive;

        // 设置物体的激活状态
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(isObjectActive);
        }
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(!isObjectActive);
        }

        // 输出当前状态
        Debug.LogWarning($"物体 {objectToActivate.name} 已设置为 {isObjectActive}，物体 {objectToDeactivate.name} 已设置为 {!isObjectActive}。");
    }
}