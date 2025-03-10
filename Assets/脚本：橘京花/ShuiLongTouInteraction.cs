using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShuiLongTouInteraction : MonoBehaviour
{
    public ParticleSystem particleEffect; // 用户自行添加的粒子效果
    public float grabDistanceThreshold = 0.5f; // 抓握距离阈值
    private bool isGrabbing = false;
    private Transform handTransform;
    private Quaternion initialRotation;
    private float rotationThreshold = 90f; // 旋转超过90度触发粒子效果

    void Update()
    {
        // 持续检测左手柄的grip按键
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out bool isGrippingLeft) && isGrippingLeft)
        {
            HandleGrab(XRNode.LeftHand);
        }
        // 持续检测右手柄的grip按键
        else if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out bool isGrippingRight) && isGrippingRight)
        {
            HandleGrab(XRNode.RightHand);
        }
        else
        {
            HandleRelease();
        }

        if (isGrabbing)
        {
            RotateObjectWithHand();
            CheckRotation();
            CheckDistance();
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
                initialRotation = transform.rotation;
            }
        }
    }

    private void HandleRelease()
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            handTransform = null;
        }
    }

    private void RotateObjectWithHand()
    {
        if (handTransform != null)
        {
            Vector3 handRotation = handTransform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, handRotation.z);
        }
    }

    private void CheckRotation()
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        float initialZRotation = initialRotation.eulerAngles.z;
        float rotationDifference = Mathf.Abs(currentRotation - initialZRotation);

        if (rotationDifference > rotationThreshold)
        {
            TriggerParticleEffect();
        }
    }

    private void CheckDistance()
    {
        if (handTransform != null && Vector3.Distance(handTransform.position, transform.position) > grabDistanceThreshold)
        {
            HandleRelease();
        }
    }

    private void TriggerParticleEffect()
    {
        if (particleEffect != null && !particleEffect.isPlaying)
        {
            particleEffect.Play();
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