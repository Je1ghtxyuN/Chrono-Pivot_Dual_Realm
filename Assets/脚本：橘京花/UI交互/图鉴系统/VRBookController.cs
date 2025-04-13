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
    public List<GameObject> bookPages;
    public int currentPageIndex = 0;

    [Header("按钮设置")]
    public Button nextPageButton;
    public Button prevPageButton;
    public Button exitButton;

    [Header("XR设置")]
    public XRRayInteractor rayInteractor;

    [Header("输入设置")]
    [SerializeField] private bool enableMouseInput = true;
    [SerializeField] private bool showDebugLogs = true;

    // 用于跟踪上次鼠标悬停的对象
    private GameObject lastHoveredObject = null;

    void Start()
    {
        // 初始化按钮事件
        if (nextPageButton != null)
        {
            nextPageButton.onClick.AddListener(GoToNextPage);
        }
        if (prevPageButton != null)
        {
            prevPageButton.onClick.AddListener(GoToPrevPage);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitBook);
        }

        ShowCurrentPage();
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

    public void GoToNextPage()
    {
        if (currentPageIndex < bookPages.Count - 1)
        {
            currentPageIndex++;
            if (showDebugLogs) Debug.Log($"切换到下一页，当前页索引: {currentPageIndex}");
            ShowCurrentPage();
        }
        else
        {
            if (showDebugLogs) Debug.Log("已是最后一页，无法继续翻页");
        }
    }

    public void GoToPrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            if (showDebugLogs) Debug.Log($"切换到上一页，当前页索引: {currentPageIndex}");
            ShowCurrentPage();
        }
        else
        {
            if (showDebugLogs) Debug.Log("已是第一页，无法继续翻页");
        }
    }

    public void ExitBook()
    {
        if (showDebugLogs) Debug.Log("正在退出书本...");
        gameObject.SetActive(false);
    }

    private void ShowCurrentPage()
    {
        for (int i = 0; i < bookPages.Count; i++)
        {
            bool isActive = i == currentPageIndex;
            bookPages[i]?.SetActive(isActive);

            if (showDebugLogs && isActive)
            {
                Debug.Log($"显示页面: {bookPages[i].name}");
            }
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
                if (showDebugLogs)
                {
                    Debug.Log($"鼠标悬停在: {currentHoveredObject.name}", currentHoveredObject);
                }
                lastHoveredObject = currentHoveredObject;
            }
        }
        else if (lastHoveredObject != null)
        {
            if (showDebugLogs)
            {
                Debug.Log("鼠标离开UI元素", lastHoveredObject);
            }
            lastHoveredObject = null;
        }

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            if (showDebugLogs)
            {
                Debug.Log("鼠标左键按下");
            }

            if (results.Count > 0)
            {
                // 只处理最顶层的UI元素
                GameObject clickedObject = results[0].gameObject;
                HandleButtonClick(clickedObject);
            }
        }
    }

    // 检测UI交互
    private void CheckUIInteraction()
    {
        // 检测射线是否对准UI按钮
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            HandleButtonClick(raycastResult.gameObject);
        }
    }

    // 处理按钮点击
    private void HandleButtonClick(GameObject clickedObject)
    {
        if (showDebugLogs)
        {
            Debug.Log($"尝试点击: {clickedObject.name}", clickedObject);
        }

        // 检查点击的是否是下一页按钮
        if (clickedObject == nextPageButton.gameObject || clickedObject.transform.IsChildOf(nextPageButton.transform))
        {
            if (nextPageButton.interactable)
            {
                if (showDebugLogs) Debug.Log("点击下一页按钮");
                GoToNextPage();
            }
        }
        // 检查点击的是否是上一页按钮
        else if (clickedObject == prevPageButton.gameObject || clickedObject.transform.IsChildOf(prevPageButton.transform))
        {
            if (prevPageButton.interactable)
            {
                if (showDebugLogs) Debug.Log("点击上一页按钮");
                GoToPrevPage();
            }
        }
        // 检查点击的是否是退出按钮
        else if (clickedObject == exitButton.gameObject || clickedObject.transform.IsChildOf(exitButton.transform))
        {
            if (exitButton.interactable)
            {
                if (showDebugLogs) Debug.Log("点击退出按钮");
                ExitBook();
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