using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UISceneSwitcher : MonoBehaviour
{
    public string sceneToLoad; // 公开字段，用于设置要切换的场景名称
    public XRRayInteractor rayInteractor; // 引用XR射线交互器

    void Update()
    {
        // 检测右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 检测左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);

        // 如果任意手柄的扳机键按下
        if (isRightTriggerPressed || isLeftTriggerPressed)
        {
            // 检测射线是否对准UI按钮
            if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
            {
                Button button = raycastResult.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    // 切换场景
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
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