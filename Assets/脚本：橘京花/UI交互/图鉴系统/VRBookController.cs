using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.Management;

public class VRBookController : MonoBehaviour
{
    [Header("页面设置")]
    public List<GameObject> bookPages; // 按顺序排列的书页图片
    public int currentPageIndex = 0; // 当前显示的页面索引

    [Header("按钮设置")]
    public Button exitButton; // 退出按钮
    public Button nextPageButton; // 下一页按钮
    public Button prevPageButton; // 上一页按钮

    [Header("交互设置")]
    public XRRayInteractor rayInteractor; // XR射线交互器
    [SerializeField] private bool enableMouseInput = true; // 是否启用鼠标输入

    // 用于跟踪上次触碰的按钮
    private GameObject lastHoveredButton = null;

    private void Start()
    {
        // 初始化按钮事件
        if (exitButton != null) exitButton.onClick.AddListener(() => OnButtonClicked(exitButton.gameObject, "退出"));
        if (nextPageButton != null) nextPageButton.onClick.AddListener(() => OnButtonClicked(nextPageButton.gameObject, "下一页"));
        if (prevPageButton != null) prevPageButton.onClick.AddListener(() => OnButtonClicked(prevPageButton.gameObject, "上一页"));

        // 初始化显示第一页
        ShowCurrentPage();
    }

    private void Update()
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
            CheckButtonInteraction(isTriggerPressed);
        }
        else
        {
            // 仅检测悬停不检测按下
            CheckButtonInteraction(false);
        }
    }

    // 修改后的检测鼠标输入方法
    private void CheckMouseInput()
    {
        // 使用更精确的检测方式
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            GameObject currentHoveredButton = null;

            foreach (var result in results)
            {
                if (result.gameObject == exitButton?.gameObject)
                {
                    currentHoveredButton = exitButton.gameObject;
                    break;
                }
                else if (result.gameObject == nextPageButton?.gameObject)
                {
                    currentHoveredButton = nextPageButton.gameObject;
                    break;
                }
                else if (result.gameObject == prevPageButton?.gameObject)
                {
                    currentHoveredButton = prevPageButton.gameObject;
                    break;
                }
            }

            // 检测悬停状态变化
            if (currentHoveredButton != lastHoveredButton)
            {
                if (currentHoveredButton != null)
                {
                    Debug.Log($"鼠标悬停在按钮: {GetButtonName(currentHoveredButton)}");
                }
                lastHoveredButton = currentHoveredButton;
            }

            // 检测鼠标点击
            if (Input.GetMouseButtonDown(0) && currentHoveredButton != null)
            {
                if (currentHoveredButton == exitButton?.gameObject)
                {
                    OnExitButtonClicked();
                }
                else if (currentHoveredButton == nextPageButton?.gameObject)
                {
                    OnNextPageButtonClicked();
                }
                else if (currentHoveredButton == prevPageButton?.gameObject)
                {
                    OnPrevPageButtonClicked();
                }
            }
        }
    }

    // 检测按钮交互
    private void CheckButtonInteraction(bool checkPress)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            GameObject hitObject = raycastResult.gameObject;

            // 检测悬停状态变化
            if (hitObject != lastHoveredButton)
            {
                if (hitObject == exitButton?.gameObject ||
                    hitObject == nextPageButton?.gameObject ||
                    hitObject == prevPageButton?.gameObject)
                {
                    Debug.Log($"手柄射线触碰到按钮: {GetButtonName(hitObject)}");
                }
                lastHoveredButton = hitObject;
            }

            // 检测按钮按下
            if (checkPress)
            {
                if (hitObject == exitButton?.gameObject)
                {
                    OnExitButtonClicked();
                }
                else if (hitObject == nextPageButton?.gameObject)
                {
                    OnNextPageButtonClicked();
                }
                else if (hitObject == prevPageButton?.gameObject)
                {
                    OnPrevPageButtonClicked();
                }
            }
        }
        else if (lastHoveredButton != null)
        {
            lastHoveredButton = null;
        }
    }

    // 获取按钮名称
    private string GetButtonName(GameObject buttonObj)
    {
        if (buttonObj == exitButton?.gameObject) return "退出按钮";
        if (buttonObj == nextPageButton?.gameObject) return "下一页按钮";
        if (buttonObj == prevPageButton?.gameObject) return "上一页按钮";
        return "未知按钮";
    }

    // 按钮点击事件
    private void OnButtonClicked(GameObject buttonObj, string buttonName)
    {
        Debug.Log($"按钮被按下: {buttonName}");
    }

    // 退出按钮点击事件
    private void OnExitButtonClicked()
    {
        OnButtonClicked(exitButton.gameObject, "退出按钮");
        gameObject.SetActive(false); // 关闭书本画布
    }

    // 下一页按钮点击事件
    private void OnNextPageButtonClicked()
    {
        OnButtonClicked(nextPageButton.gameObject, "下一页按钮");
        if (currentPageIndex < bookPages.Count - 1)
        {
            currentPageIndex++;
            ShowCurrentPage();
        }
    }

    // 上一页按钮点击事件
    private void OnPrevPageButtonClicked()
    {
        OnButtonClicked(prevPageButton.gameObject, "上一页按钮");
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowCurrentPage();
        }
    }

    // 显示当前页
    private void ShowCurrentPage()
    {
        // 隐藏所有页面
        foreach (var page in bookPages)
        {
            if (page != null) page.SetActive(false);
        }

        // 显示当前页面
        if (currentPageIndex >= 0 && currentPageIndex < bookPages.Count && bookPages[currentPageIndex] != null)
        {
            bookPages[currentPageIndex].SetActive(true);
        }

        // 更新按钮状态
        UpdateButtonStates();
    }

    // 更新按钮状态
    private void UpdateButtonStates()
    {
        if (prevPageButton != null)
        {
            prevPageButton.interactable = currentPageIndex > 0;
        }

        if (nextPageButton != null)
        {
            nextPageButton.interactable = currentPageIndex < bookPages.Count - 1;
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

    private void OnDestroy()
    {
        // 移除按钮事件监听
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();
        if (nextPageButton != null) nextPageButton.onClick.RemoveAllListeners();
        if (prevPageButton != null) prevPageButton.onClick.RemoveAllListeners();
    }
}