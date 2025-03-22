using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ShuiLongTouInteractionSimplified : MonoBehaviour
{
    public ParticleSystem particleEffect; // 用户自行添加的粒子效果
    public Animator animator; // 水龙头的动画控制器

    private bool isGrabbing = false;
    private bool isRotatingForward = false; // 是否正在正向旋转
    private XRSimpleInteractable parentInteractable; // 父物体的交互组件

    private XRRayInteractor rayInteractor; // 射线组件
    [Header("被滴穿的物体")]
    public DisAppearController disAppearController; // 水滴穿过后消失的物体

    void Start()
    {
        // 初始化时关闭粒子效果
        if (particleEffect != null)
        {
            particleEffect.Stop();
        }

        // 获取父物体的 XRSimpleInteractable 组件
        parentInteractable = transform.parent?.GetComponent<XRSimpleInteractable>();
        if (parentInteractable == null)
        {
            Debug.LogError("Parent XRSimpleInteractable component not found on this object!");
        }

        // 获取射线组件
        rayInteractor = FindObjectOfType<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("XRRayInteractor not found in the scene!");
        }

        // 确保 Animator 已正确设置
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator not found on the object!");
            }
        }
        disAppearController = FindObjectOfType<DisAppearController>();
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
        // 检查父物体是否被高亮（即是否被射线选中）
        bool isParentHighlighted = parentInteractable != null && parentInteractable.isHovered;

        // 如果父物体被高亮，输出调试信息
        if (isParentHighlighted)
        {
            Debug.LogWarning("父物体被高亮，射线选中。");
        }

        // 持续检测左手柄的 grip 按键
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.grip, out float leftGripValue) && leftGripValue > 0.5f)
        {
            Debug.LogWarning("左手柄抓握键按下。");
            HandleGrab(isParentHighlighted);
        }
        // 持续检测右手柄的 grip 按键
        else if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.grip, out float rightGripValue) && rightGripValue > 0.5f)
        {
            Debug.LogWarning("右手柄抓握键按下。");
            HandleGrab(isParentHighlighted);
        }
        else
        {
            HandleRelease();
        }
    }

    private void HandleGrab(bool isParentHighlighted)
    {
        if (!isGrabbing && isParentHighlighted)
        {
            isGrabbing = true;

            // 切换旋转方向
            isRotatingForward = !isRotatingForward;

            // 触发动画
            if (isRotatingForward)
            {
                animator.SetTrigger("RotateForward");
                if (particleEffect != null && !particleEffect.isPlaying)
                {
                    particleEffect.Play();
                    Debug.LogWarning("粒子效果已开启。");
                    disAppearController.DisAppear();
                }
            }
            else
            {
                animator.SetTrigger("RotateBackward");
                if (particleEffect != null && particleEffect.isPlaying)
                {
                    particleEffect.Stop();
                    Debug.LogWarning("粒子效果已关闭。");
                }
            }

            Debug.LogWarning("抓握开始，旋转方向：" + (isRotatingForward ? "正向" : "反向"));
        }
    }

    private void HandleRelease()
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            Debug.LogWarning("抓握结束。");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 恢复场景状态
        SceneStateManager.Instance.RestoreSceneState(scene.name);
    }
}