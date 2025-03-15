using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.XR;

public class SceneSwitchOnSelectPro : MonoBehaviour
{
    public Transform player; // 玩家的位置（通常是 XR Origin 或 Camera）
    public string targetSceneName = "01n"; // 要切换的目标场景名称

    private XRRayInteractor rightRayInteractor; // 右手柄射线组件
    private XRRayInteractor leftRayInteractor;  // 左手柄射线组件

    private XRBaseInteractable interactable; // 当前物体的交互组件

    private bool isSceneSwitchBlocked = false; // 是否禁止切换场景
    private float sceneSwitchBlockTimer = 0f; // 禁止切换场景的计时器
    private const float sceneSwitchBlockDuration = 2f; // 禁止切换场景的时间（2秒）

    void Start()
    {
        // 获取当前物体的 XRBaseInteractable 组件
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("XRBaseInteractable component not found on this object!");
        }

        // 获取左右手柄的射线组件
        rightRayInteractor = GetRayInteractor(XRNode.RightHand);
        leftRayInteractor = GetRayInteractor(XRNode.LeftHand);

        if (rightRayInteractor == null)
        {
            Debug.LogError("Right XRRayInteractor not found in the scene!");
        }
        if (leftRayInteractor == null)
        {
            Debug.LogError("Left XRRayInteractor not found in the scene!");
        }

        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // 如果禁止切换场景，更新计时器
        if (isSceneSwitchBlocked)
        {
            sceneSwitchBlockTimer -= Time.deltaTime;
            if (sceneSwitchBlockTimer <= 0f)
            {
                isSceneSwitchBlocked = false; // 计时器结束，允许切换场景
                Debug.LogWarning("禁止切换场景已解除，可以再次切换场景。");
            }
        }

        // 检查物体是否被高亮（即是否被射线选中）
        bool isHighlighted = interactable.isHovered;

        // 如果物体被高亮，输出调试信息
        if (isHighlighted)
        {
            Debug.LogWarning("物体被高亮，射线选中。");
        }

        // 检查右手柄的扳机键是否按下
        bool isRightTriggerPressed = IsTriggerPressed(XRNode.RightHand);

        // 如果右手柄扳机键按下，输出调试信息
        if (isRightTriggerPressed)
        {
            Debug.LogWarning("右手柄扳机键按下。");
        }

        // 检查左手柄的扳机键是否按下
        bool isLeftTriggerPressed = IsTriggerPressed(XRNode.LeftHand);

        // 如果左手柄扳机键按下，输出调试信息
        if (isLeftTriggerPressed)
        {
            Debug.LogWarning("左手柄扳机键按下。");
        }

        // 如果满足所有条件且未禁止切换场景，切换场景
        if (isHighlighted && (isRightTriggerPressed || isLeftTriggerPressed) && !isSceneSwitchBlocked)
        {
            Debug.LogWarning("条件满足，正在切换场景...");

            // 保存当前场景的状态
            SceneStateManager.Instance.SaveCurrentSceneState();

            // 加载新场景
            SceneManager.LoadScene(targetSceneName);

            // 切换场景后禁止再次切换场景
            isSceneSwitchBlocked = true;
            sceneSwitchBlockTimer = sceneSwitchBlockDuration;
            Debug.LogWarning("切换场景成功，2 秒内禁止再次切换场景。");
        }
    }

    // 场景加载后的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 恢复目标场景的状态
        SceneStateManager.Instance.RestoreSceneState(scene.name);

        // 手动恢复玩家对象的状态
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            SceneStateManager.Instance.RestorePlayerState(player);
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

    // 获取指定手柄的射线组件
    private XRRayInteractor GetRayInteractor(XRNode handNode)
    {
        XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();
        foreach (XRRayInteractor rayInteractor in rayInteractors)
        {
            if (rayInteractor.gameObject.name.Contains(handNode == XRNode.RightHand ? "Right" : "Left"))
            {
                return rayInteractor;
            }
        }
        return null;
    }
}