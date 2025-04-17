using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

[RequireComponent(typeof(Button))]
public class SimpleSceneSwitchButtonSceneSwitch : MonoBehaviour
{
    [Header("场景设置")]
    public string sceneToLoad; // 要加载的场景名称

    [Header("XR设置")]
    public XRRayInteractor rayInteractor; // XR射线交互器

    [Header("交互设置")]
    public float hoverScale = 1.1f; // 悬停时的缩放比例
    public bool enableMouseInEditor = true; // 在编辑器中启用鼠标交互

    private Vector3 originalScale;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;

        // 自动查找射线交互器（如果没有指定）
        if (rayInteractor == null)
        {
            rayInteractor = FindObjectOfType<XRRayInteractor>();
        }

        // 添加点击事件监听
        button.onClick.AddListener(OnButtonClicked);
    }

    void Update()
    {
        CheckVRHover();

        // 在编辑器或非XR环境下检查鼠标交互
        if (enableMouseInEditor && (Application.isEditor || !IsXREnabled()))
        {
            CheckMouseHover();
        }
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"切换场景到: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("未设置要加载的场景名称!");
        }
    }

    private void CheckVRHover()
    {
        if (rayInteractor == null || !button.interactable) return;

        bool isHovering = false;

        // 检查射线是否对准这个按钮
        if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            isHovering = raycastResult.gameObject == gameObject ||
                        raycastResult.gameObject.transform.IsChildOf(transform);
        }

        // 悬停效果
        transform.localScale = isHovering ? originalScale * hoverScale : originalScale;

        // 检查扳机键按下
        if (isHovering && IsTriggerPressed())
        {
            button.onClick.Invoke();
        }
    }

    private void CheckMouseHover()
    {
        if (!button.interactable) return;

        // 创建指针事件数据
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // 检查鼠标悬停
        bool isHovering = false;
        if (results.Count > 0)
        {
            isHovering = results[0].gameObject == gameObject ||
                        results[0].gameObject.transform.IsChildOf(transform);
        }

        // 悬停效果
        transform.localScale = isHovering ? originalScale * hoverScale : originalScale;

        // 检查鼠标点击
        if (isHovering && Input.GetMouseButtonDown(0))
        {
            button.onClick.Invoke();
        }
    }

    private bool IsTriggerPressed()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);

        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue) && triggerValue)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsXREnabled()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings == null) return false;

        var xrManager = xrSettings.Manager;
        if (xrManager == null) return false;

        return xrManager.activeLoader != null;
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("场景名称未设置!", this);
        }
    }
}