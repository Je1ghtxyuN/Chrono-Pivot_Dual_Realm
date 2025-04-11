using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.Management;

public class TuJianController : MonoBehaviour
{
    [Header("画布控制")]
    public List<GameObject> canvasesToClose; // 需要关闭的画布列表
    public GameObject canvasToOpen; // 需要打开的画布

    [Header("交互设置")]
    public XRRayInteractor rayInteractor; // XR射线交互器
    [SerializeField] private bool enableMouseInput = true; // 是否启用鼠标输入

    private Button button; // 当前按钮组件

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

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
        if (xrSettings == null) return false;
        var xrManager = xrSettings.Manager;
        if (xrManager == null) return false;
        return xrManager.activeLoader != null;
    }

    // 检测VR手柄输入
    private void CheckVRInput()
    {
        // 检测任意手柄的扳机键是否按下
        bool isTriggerPressed = IsTriggerPressed(XRNode.RightHand) || IsTriggerPressed(XRNode.LeftHand);

        if (isTriggerPressed)
        {
            CheckUIInteraction();
        }
    }

    // 检测鼠标输入
    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 检查是否点击了这个按钮
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0 && results[0].gameObject == gameObject)
            {
                OnButtonClicked();
            }
        }
    }

    // 检测UI交互
    private void CheckUIInteraction()
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            if (raycastResult.gameObject == gameObject)
            {
                OnButtonClicked();
            }
        }
    }

    // 按钮点击事件
    private void OnButtonClicked()
    {
        // 关闭指定画布
        foreach (var canvas in canvasesToClose)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }

        // 打开指定画布
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(true);
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

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}