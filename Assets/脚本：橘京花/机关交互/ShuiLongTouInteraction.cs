//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR;
//using UnityEngine.XR.Interaction.Toolkit;

//public class ShuiLongTouInteraction : MonoBehaviour
//{
//    public ParticleSystem particleEffect; // 用户自行添加的粒子效果
//    public float grabDistanceThreshold = 0.5f; // 抓握距离阈值

//    private bool isGrabbing = false;
//    private Transform handTransform;
//    private Quaternion initialRotation; // 物体的初始旋转
//    private Quaternion initialHandRotation; // 手柄的初始旋转
//    private float rotationThreshold = 180f; // 旋转超过180度触发粒子效果

//    private XRRayInteractor rayInteractor; // 射线组件

//    void Start()
//    {
//        // 初始化时关闭粒子效果
//        if (particleEffect != null)
//        {
//            particleEffect.Stop();
//        }

//        // 获取射线组件
//        rayInteractor = FindObjectOfType<XRRayInteractor>();
//        if (rayInteractor == null)
//        {
//            Debug.LogError("XRRayInteractor not found in the scene!");
//        }
//    }

//    void Update()
//    {
//        // 检查射线是否射中水龙头
//        CheckRayHit();

//        // 持续检测左手柄的grip按键
//        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.grip, out float leftGripValue) && leftGripValue > 0.5f)
//        {
//            HandleGrab(XRNode.LeftHand);
//        }
//        // 持续检测右手柄的grip按键
//        else if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.grip, out float rightGripValue) && rightGripValue > 0.5f)
//        {
//            HandleGrab(XRNode.RightHand);
//        }
//        else
//        {
//            HandleRelease();
//        }

//        if (isGrabbing)
//        {
//            RotateObjectWithHand();
//            CheckRotation();
//            CheckDistance();
//        }
//    }

//    private void CheckRayHit()
//    {
//        if (rayInteractor != null)
//        {
//            // 获取射线击中的物体
//            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
//            {
//                // 检查击中的物体是否是当前物体
//                if (hit.transform == transform)
//                {
//                    Debug.Log("Ray hit object: " + hit.transform.name);
//                }
//            }
//        }
//    }

//    private void HandleGrab(XRNode handNode)
//    {
//        if (!isGrabbing)
//        {
//            handTransform = GetHandTransform(handNode);
//            if (handTransform != null && Vector3.Distance(handTransform.position, transform.position) <= grabDistanceThreshold)
//            {
//                isGrabbing = true;
//                initialRotation = transform.rotation; // 记录物体的初始旋转
//                initialHandRotation = handTransform.rotation; // 记录手柄的初始旋转

//                Debug.Log("Grab started. Initial hand rotation: " + initialHandRotation.eulerAngles);
//                Debug.Log("Initial object rotation: " + initialRotation.eulerAngles);
//            }
//        }
//    }

//    private void HandleRelease()
//    {
//        if (isGrabbing)
//        {
//            isGrabbing = false;
//            handTransform = null;

//            Debug.Log("Grab released.");
//        }
//    }

//    private void RotateObjectWithHand()
//    {
//        if (handTransform != null)
//        {
//            // 计算手柄的旋转变化
//            Quaternion handRotationDelta = handTransform.rotation * Quaternion.Inverse(initialHandRotation);

//            // 将旋转变化应用到物体上
//            transform.rotation = initialRotation * handRotationDelta;

//            Debug.Log("Hand rotation: " + handTransform.rotation.eulerAngles);
//            Debug.Log("Object rotation: " + transform.rotation.eulerAngles);
//        }
//    }

//    private void CheckRotation()
//    {
//        float currentRotation = transform.rotation.eulerAngles.z;
//        float initialZRotation = initialRotation.eulerAngles.z;
//        float rotationDifference = Mathf.Abs(currentRotation - initialZRotation);

//        Debug.Log("Rotation difference: " + rotationDifference);

//        if (rotationDifference > rotationThreshold)
//        {
//            TriggerParticleEffect();
//        }
//    }

//    private void CheckDistance()
//    {
//        if (handTransform != null && Vector3.Distance(handTransform.position, transform.position) > grabDistanceThreshold)
//        {
//            HandleRelease();
//        }
//    }

//    private void TriggerParticleEffect()
//    {
//        if (particleEffect != null && !particleEffect.isPlaying)
//        {
//            particleEffect.Play();
//            Debug.Log("Particle effect triggered.");
//        }
//    }

//    private Transform GetHandTransform(XRNode handNode)
//    {
//        InputDevice handDevice = InputDevices.GetDeviceAtXRNode(handNode);
//        if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) &&
//            handDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
//        {
//            GameObject hand = new GameObject("Hand");
//            hand.transform.position = position;
//            hand.transform.rotation = rotation;
//            return hand.transform;
//        }
//        return null;
//    }
//}