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

    [Header("被滴穿的物体")]
    public DisAppearController disAppearController; // 水滴穿过后消失的物体

    void Start()
    {
        // 初始化时关闭粒子效果
        if (particleEffect != null)
        {
            particleEffect.Stop();
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
        // 持续检测左手柄的 grip 按键
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.grip, out float leftGripValue) && leftGripValue > 0.5f)
        {
            Debug.LogWarning("左手柄抓握键按下。");
            HandleGrab();
        }
        // 持续检测右手柄的 grip 按键
        else if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.grip, out float rightGripValue) && rightGripValue > 0.5f)
        {
            Debug.LogWarning("右手柄抓握键按下。");
            HandleGrab();
        }
        else
        {
            HandleRelease();
        }
    }

    private void HandleGrab()
    {
        if (!isGrabbing)
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