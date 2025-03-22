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
        Debug.Log($"右手柄扳机键状态: {isRightTriggerPressed}");

        // 检测左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);
        Debug.Log($"左手柄扳机键状态: {isLeftTriggerPressed}");

        // 如果任意手柄的扳机键按下
        if (isRightTriggerPressed || isLeftTriggerPressed)
        {
            Debug.Log("检测到手柄扳机键按下，开始检测射线是否对准UI按钮...");

            // 检测射线是否对准UI按钮
            if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
            {
                Debug.Log($"射线检测到UI对象: {raycastResult.gameObject.name}");

                Button button = raycastResult.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    Debug.Log($"检测到按钮: {button.name}，准备切换场景...");
                    // 切换场景
                    SceneManager.LoadScene(sceneToLoad);
                    Debug.Log($"场景切换成功，加载场景: {sceneToLoad}");
                }
                else
                {
                    Debug.LogWarning($"射线检测到的对象 {raycastResult.gameObject.name} 不是按钮，无法切换场景。");
                }
            }
            else
            {
                Debug.LogWarning("射线未检测到任何UI对象。");
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
            else
            {
                Debug.LogWarning($"无法获取 {handNode} 手柄的扳机键状态。");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"未找到 {handNode} 手柄设备。");
            return false;
        }
    }
}