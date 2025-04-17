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
            CheckSceneExists();
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 警告: sceneToLoad 未设置!");
        }
    }

    // 检查场景是否存在
    private void CheckSceneExists()
    {
        bool sceneExists = IsSceneInBuildSettings(sceneToLoad);
        Debug.Log(sceneExists ? $"[UISceneSwitcher] 场景 '{sceneToLoad}' 存在于Build Settings" :
                              $"[UISceneSwitcher] 警告: 场景 '{sceneToLoad}' 不存在于Build Settings!");
    }

    // 检查场景是否存在于Build Settings中
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneName)
            {
                return true;
            }
        }
        return false;
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
        // 如果任意手柄的扳机键按下
        if (IsTriggerPressed())
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
                        // 启动场景切换
                        SwitchScene();
                    }
                }
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
                    // 启动场景切换
                    SwitchScene();
                }
            }
        }
    }

    // 场景切换主逻辑
    private void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"[UISceneSwitcher] 正在尝试切换场景到: {sceneToLoad}");

            // 检查场景是否存在
            if (IsSceneInBuildSettings(sceneToLoad))
            {
                // 启动异步加载场景
                StartCoroutine(LoadSceneAsync());
            }
            else
            {
                Debug.LogError($"[UISceneSwitcher] 场景加载失败: 场景 '{sceneToLoad}' 不存在于Build Settings中!");
            }
        }
        else
        {
            Debug.LogWarning("[UISceneSwitcher] 未设置要加载的场景名称!");
        }
    }

    // 检查手柄的扳机键是否按下
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

    // 异步加载场景
    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("[UISceneSwitcher] 开始异步加载场景: " + sceneToLoad);

        // 1. 禁用按钮
        DisableButtons();

        // 2. 更改天空盒
        if (loadingSkybox != null)
        {
            RenderSettings.skybox = loadingSkybox;
            DynamicGI.UpdateEnvironment();
        }

        // 3. 显示加载界面
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);

            // 初始化进度条
            if (progressBar != null) progressBar.value = 0;
            if (progressText != null) progressText.text = "0%";
        }

        // 4. 开始异步加载
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // 5. 更新进度
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // 更新UI
            if (progressBar != null) progressBar.value = progress;
            if (progressText != null) progressText.text = (progress * 100).ToString("F0") + "%";

            // 加载接近完成
            if (asyncLoad.progress >= 0.9f)
            {
                // 恢复原始天空盒
                if (originalSkybox != null)
                {
                    RenderSettings.skybox = originalSkybox;
                    DynamicGI.UpdateEnvironment();
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log($"[UISceneSwitcher] 成功加载场景: {sceneToLoad}");
    }

    // 禁用按钮
    private void DisableButtons()
    {
        if (buttonsToDisable != null)
        {
            foreach (var canvas in buttonsToDisable)
            {
                if (canvas != null) canvas.enabled = false;
            }
        }
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