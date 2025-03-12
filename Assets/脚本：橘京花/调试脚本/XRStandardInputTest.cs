using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRStandardInputTest : MonoBehaviour
{
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    void Start()
    {
        InitializeDevices();
    }

    void Update()
    {
        if (!leftHandDevice.isValid || !rightHandDevice.isValid)
        {
            InitializeDevices();
        }

        CheckLeftHandInput();
        CheckRightHandInput();
    }

    private void InitializeDevices()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count > 0)
        {
            leftHandDevice = leftHandDevices[0];
            Debug.Log("Left hand device initialized: " + leftHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Left hand device not found!");
        }

        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightHandDevice = rightHandDevices[0];
            Debug.Log("Right hand device initialized: " + rightHandDevice.name);
        }
        else
        {
            Debug.LogWarning("Right hand device not found!");
        }
    }

    private void CheckLeftHandInput()
    {
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            Debug.Log("Left Trigger: " + triggerValue);
        }
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripValue))
        {
            Debug.Log("Left Grip: " + gripValue);
        }
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue))
        {
            Debug.Log("Left Primary Button: " + primaryValue);
        }
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue))
        {
            Debug.Log("Left Secondary Button: " + secondaryValue);
        }
    }

    private void CheckRightHandInput()
    {
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            Debug.Log("Right Trigger: " + triggerValue);
        }
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripValue))
        {
            Debug.Log("Right Grip: " + gripValue);
        }
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue))
        {
            Debug.Log("Right Primary Button: " + primaryValue);
        }
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue))
        {
            Debug.Log("Right Secondary Button: " + secondaryValue);
        }
    }
}