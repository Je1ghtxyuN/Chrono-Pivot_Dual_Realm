using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.Management;
using System.Collections;

public class UISceneSwitcher : MonoBehaviour
{
    [Header("场景设置")]
    public string sceneToLoad; // 要切换的场景名称
    public Material loadingSkybox; // 加载时使用的天空盒材质

    [Header("XR设置")]
    public XRRayInteractor rayInteractor; // XR射线交互器

    [Header("UI设置")]
    [SerializeField] private bool enableMouseInput = true; // 是否启用鼠标输入
    [SerializeField] private bool debugMouseInput = true; // 是否输出鼠标调试信息
    public Canvas[] buttonsToDisable; // 需要在加载时禁用的按钮Canvas数组
    public GameObject loadingScreen; // 加载界面对象
    public Slider progressBar; // 进度条Slider
    public Text progressText; // 进度百分比文本(可选)

    // 用于跟踪上次鼠标悬停的对象
    private GameObject lastHoveredObject = null;
    private Material originalSkybox; // 保存原始天空盒

    void Start()
    {
        Debug.Log("[UISceneSwitcher] Start() 初始化开始");

        // 保存原始天空盒
        originalSkybox = RenderSettings.skybox;
        Debug.Log($"[UISceneSwitcher] 原始天空盒保存: {(originalSkybox != null ? originalSkybox.name : "null")}");

        // 初始隐藏加载界面
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
            Debug.Log("[UISceneSwitcher] 加载界面初始隐藏");
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: loadingScreen 未赋值!");
        }

        // 检查场景是否存在
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneToLoad)
                {
                    sceneExists = true;
                    break;
                }
            }
            Debug.Log(sceneExists ? $"[UISceneSwitcher] 场景 '{sceneToLoad}' 存在于Build Settings" :
                                  $"[UISceneSwitcher] 警告: 场景 '{sceneToLoad}' 不存在于Build Settings!");
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: sceneToLoad 未设置!");
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
                        // 启动异步加载场景
                        StartCoroutine(LoadSceneAsync());
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
                    // 启动异步加载场景
                    StartCoroutine(LoadSceneAsync());
                }
            }
        }
    }

    // 异步加载场景
    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("[UISceneSwitcher] 开始异步加载场景: " + sceneToLoad);

        // ===== 1. 禁用按钮 =====
        Debug.Log("[UISceneSwitcher] 正在禁用按钮...");
        if (buttonsToDisable != null)
        {
            Debug.Log($"[UISceneSwitcher] 找到 {buttonsToDisable.Length} 个需要禁用的按钮");
            for (int i = 0; i < buttonsToDisable.Length; i++)
            {
                if (buttonsToDisable[i] != null)
                {
                    Debug.Log($"[UISceneSwitcher] 禁用按钮: {buttonsToDisable[i].name}");
                    buttonsToDisable[i].enabled = false;
                }
                else
                {
                    Debug.LogWarning($"[UISceneSwitcher] 警告: buttonsToDisable[{i}] 为null!");
                }
            }
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: buttonsToDisable 为null");
        }

        // ===== 2. 更改天空盒 =====
        Debug.Log("[UISceneSwitcher] 正在更改天空盒...");
        if (loadingSkybox != null)
        {
            Debug.Log($"[UISceneSwitcher] 设置加载天空盒: {loadingSkybox.name}");
            RenderSettings.skybox = loadingSkybox;
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: loadingSkybox 未设置，保持原天空盒");
        }

        // ===== 3. 显示加载界面 =====
        Debug.Log("[UISceneSwitcher] 正在显示加载界面...");
        if (loadingScreen != null)
        {
            Debug.Log($"[UISceneSwitcher] 激活加载界面: {loadingScreen.name}");
            loadingScreen.SetActive(true);

            // ===== 4. 初始化进度条 =====
            if (progressBar != null)
            {
                Debug.Log($"[UISceneSwitcher] 重置进度条: {progressBar.name}");
                progressBar.value = 0;
            }
            else
            {
                Debug.LogWarning("[UISceneSwitcher] 警告: progressBar 未设置!");
            }

            if (progressText != null)
            {
                progressText.text = "0%";
            }
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: loadingScreen 未设置!");
            yield break; // 终止协程
        }

        // ===== 5. 开始异步加载 =====
        Debug.Log($"[UISceneSwitcher] 开始加载场景: {sceneToLoad}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        if (asyncLoad == null)
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: 场景加载失败，请检查场景名称!");
            yield break;
        }

        asyncLoad.allowSceneActivation = false;
        Debug.Log("[UISceneSwitcher] 场景加载已开始，等待完成...");

        // ===== 6. 更新进度 =====
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[UISceneSwitcher] 当前加载进度: {progress * 100:F1}%");

            // 更新进度条
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            // 更新进度文本
            if (progressText != null)
            {
                progressText.text = (progress * 100).ToString("F0") + "%";
            }

            // 加载接近完成
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("[UISceneSwitcher] 场景加载接近完成(90%)");

                // 恢复原始天空盒
                if (originalSkybox != null)
                {
                    Debug.Log($"[UISceneSwitcher] 恢复原始天空盒: {originalSkybox.name}");
                    RenderSettings.skybox = originalSkybox;
                    DynamicGI.UpdateEnvironment();
                }

                Debug.Log("[UISceneSwitcher] 允许场景激活");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("[UISceneSwitcher] 场景加载完成!");
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

    // 编辑器验证
    void OnValidate()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("场景名称未设置!", this);
        }

        if (loadingScreen == null)
        {
            Debug.LogWarning("加载界面未赋值!", this);
        }

        if (progressBar == null)
        {
            Debug.LogWarning("进度条未赋值!", this);
        }
    }
}