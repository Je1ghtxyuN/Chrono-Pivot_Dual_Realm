using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.Management;

public class UISceneSwitcher : MonoBehaviour
{
    public string sceneToLoad; // 要切换的场景名称
    public XRRayInteractor rayInteractor; // XR射线交互器

    [Header("输入设置")]
    [SerializeField] private bool enableMouseInput = true; // 是否启用鼠标输入
    [SerializeField] private bool debugMouseInput = true; // 是否输出鼠标调试信息

    // 用于跟踪上次鼠标悬停的对象
    private GameObject lastHoveredObject = null;

    void Update()
    {
        // VR手柄输入检测
        CheckVRInput();

        // 鼠标输入检测（仅在编辑器或非XR环境下）
        if (enableMouseInput && (Application.isEditor || !IsXREnabled()))
        {
            CheckMouseInput();
        }
    }

    // 检测XR是否启用
    private bool IsXREnabled()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings == null)
            return false;

        var xrManager = xrSettings.Manager;
        if (xrManager == null)
            return false;

        var xrLoader = xrManager.activeLoader;
        return xrLoader != null;
    }

    // 检测VR手柄输入
    private void CheckVRInput()
    {
        // 检测右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 检测左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);

        // 如果任意手柄的扳机键按下
        if (isRightTriggerPressed || isLeftTriggerPressed)
        {
            CheckUIInteraction();
        }
    }

    // 检测鼠标输入
    private void CheckMouseInput()
    {
        // 创建指针事件数据
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // 鼠标悬停检测
        if (results.Count > 0)
        {
            GameObject currentHoveredObject = results[0].gameObject;

            // 如果悬停对象发生变化
            if (currentHoveredObject != lastHoveredObject)
            {
                if (debugMouseInput)
                {
                    Debug.Log($"鼠标悬停在: {currentHoveredObject.name}", currentHoveredObject);
                }
                lastHoveredObject = currentHoveredObject;
            }
        }
        else if (lastHoveredObject != null)
        {
            if (debugMouseInput)
            {
                Debug.Log("鼠标离开UI元素", lastHoveredObject);
            }
            lastHoveredObject = null;
        }

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            if (debugMouseInput)
            {
                Debug.Log("鼠标左键按下");
            }

            if (results.Count > 0)
            {
                // 只处理最顶层的UI元素
                GameObject clickedObject = results[0].gameObject;

                if (debugMouseInput)
                {
                    Debug.Log($"尝试点击: {clickedObject.name}", clickedObject);
                }

                // 检查点击的是否是这个按钮
                if (clickedObject == gameObject || clickedObject.transform.IsChildOf(transform))
                {
                    Button button = GetComponent<Button>();
                    if (button != null && button.interactable)
                    {
                        if (debugMouseInput)
                        {
                            Debug.Log($"成功点击按钮: {gameObject.name}, 加载场景: {sceneToLoad}", gameObject);
                        }
                        // 切换场景
                        SceneManager.LoadScene(sceneToLoad);
                    }
                    else if (debugMouseInput)
                    {
                        Debug.Log($"找到按钮但不可交互: {gameObject.name}", gameObject);
                    }
                }
                else if (debugMouseInput)
                {
                    Debug.Log($"点击的不是目标按钮: {clickedObject.name}", clickedObject);
                }
            }
            else if (debugMouseInput)
            {
                Debug.Log("鼠标点击但没有命中任何UI元素");
            }
        }
    }

    // 检测UI交互
    private void CheckUIInteraction()
    {
        // 检测射线是否对准UI按钮
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            // 检查射线击中的是否是这个按钮
            if (raycastResult.gameObject == gameObject || raycastResult.gameObject.transform.IsChildOf(transform))
            {
                Button button = GetComponent<Button>();
                if (button != null && button.interactable)
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