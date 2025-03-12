using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PICOInputTest : MonoBehaviour
{
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    // 记录上一次的按键状态
    private bool lastLeftMenuButtonState = false;
    private bool lastLeftTriggerButtonState = false;
    private bool lastLeftGripButtonState = false;
    private bool lastLeftJoystickClickState = false;
    private bool lastLeftPrimaryButtonState = false;
    private bool lastLeftSecondaryButtonState = false;

    private bool lastRightMenuButtonState = false;
    private bool lastRightTriggerButtonState = false;
    private bool lastRightGripButtonState = false;
    private bool lastRightJoystickClickState = false;
    private bool lastRightPrimaryButtonState = false;
    private bool lastRightSecondaryButtonState = false;

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
        CheckButtonState(leftHandDevice, CommonUsages.menuButton, ref lastLeftMenuButtonState, "Left Menu Button");
        CheckButtonState(leftHandDevice, CommonUsages.triggerButton, ref lastLeftTriggerButtonState, "Left Trigger Button");
        CheckButtonState(leftHandDevice, CommonUsages.gripButton, ref lastLeftGripButtonState, "Left Grip Button");
        CheckButtonState(leftHandDevice, CommonUsages.primary2DAxisClick, ref lastLeftJoystickClickState, "Left Joystick Click");
        CheckButtonState(leftHandDevice, CommonUsages.primaryButton, ref lastLeftPrimaryButtonState, "Left Primary Button (X/A)");
        CheckButtonState(leftHandDevice, CommonUsages.secondaryButton, ref lastLeftSecondaryButtonState, "Left Secondary Button (Y/B)");
    }

    private void CheckRightHandInput()
    {
        CheckButtonState(rightHandDevice, CommonUsages.menuButton, ref lastRightMenuButtonState, "Right Menu Button");
        CheckButtonState(rightHandDevice, CommonUsages.triggerButton, ref lastRightTriggerButtonState, "Right Trigger Button");
        CheckButtonState(rightHandDevice, CommonUsages.gripButton, ref lastRightGripButtonState, "Right Grip Button");
        CheckButtonState(rightHandDevice, CommonUsages.primary2DAxisClick, ref lastRightJoystickClickState, "Right Joystick Click");
        CheckButtonState(rightHandDevice, CommonUsages.primaryButton, ref lastRightPrimaryButtonState, "Right Primary Button (X/A)");
        CheckButtonState(rightHandDevice, CommonUsages.secondaryButton, ref lastRightSecondaryButtonState, "Right Secondary Button (Y/B)");
    }

    private void CheckButtonState(InputDevice device, InputFeatureUsage<bool> buttonFeature, ref bool lastState, string buttonName)
    {
        if (device.TryGetFeatureValue(buttonFeature, out bool currentState))
        {
            if (currentState != lastState)
            {
                Debug.Log(buttonName + ": " + currentState);
                lastState = currentState;
            }
        }
    }
}