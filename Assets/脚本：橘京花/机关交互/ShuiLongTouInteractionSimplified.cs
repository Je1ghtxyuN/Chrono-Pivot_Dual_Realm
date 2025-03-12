using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ShuiLongTouInteractionSimplified : MonoBehaviour
{
    public ParticleSystem particleEffect; // 用户自行添加的粒子效果
    public float grabDistanceThreshold = 0.5f; // 抓握距离阈值
    public Animator animator; // 水龙头的动画控制器

    private bool isGrabbing = false;
    private bool isRotatingForward = false; // 是否正在正向旋转
    private Transform handTransform;

    private XRRayInteractor rayInteractor; // 射线组件

    void Start()
    {
        // 初始化时关闭粒子效果
        if (particleEffect != null)
        {
            particleEffect.Stop();
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
    }

    void Update()
    {
        // 检查射线是否射中水龙头
        CheckRayHit();

        // 持续检测左手柄的grip按键
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.grip, out float leftGripValue) && leftGripValue > 0.5f)
        {
            HandleGrab(XRNode.LeftHand);
        }
        // 持续检测右手柄的grip按键
        else if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.grip, out float rightGripValue) && rightGripValue > 0.5f)
        {
            HandleGrab(XRNode.RightHand);
        }
        else
        {
            HandleRelease();
        }
    }

    private void CheckRayHit()
    {
        if (rayInteractor != null)
        {
            // 获取射线击中的物体
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // 检查击中的物体是否是当前物体
                if (hit.transform == transform)
                {
                    Debug.Log("Ray hit object: " + hit.transform.name);
                }
            }
        }
    }

    private void HandleGrab(XRNode handNode)
    {
        if (!isGrabbing)
        {
            handTransform = GetHandTransform(handNode);
            if (handTransform != null && Vector3.Distance(handTransform.position, transform.position) <= grabDistanceThreshold)
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
                        Debug.Log("Particle effect started.");
                    }
                }
                else
                {
                    animator.SetTrigger("RotateBackward");
                    if (particleEffect != null && particleEffect.isPlaying)
                    {
                        particleEffect.Stop();
                        Debug.Log("Particle effect stopped.");
                    }
                }

                Debug.Log("Grab started. Rotating forward: " + isRotatingForward);
            }
        }
    }

    private void HandleRelease()
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            handTransform = null;

            Debug.Log("Grab released.");
        }
    }

    private Transform GetHandTransform(XRNode handNode)
    {
        InputDevice handDevice = InputDevices.GetDeviceAtXRNode(handNode);
        if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) &&
            handDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            GameObject hand = new GameObject("Hand");
            hand.transform.position = position;
            hand.transform.rotation = rotation;
            return hand.transform;
        }
        return null;
    }
}