using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class HandAnimatorManagerVR : MonoBehaviour
{
    public StateModel[] leftStateModels;
    public StateModel[] rightStateModels;
    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private int leftCurrentState = 100;
    private int rightCurrentState = 100;
    private int leftLastState = -1;
    private int rightLastState = -1;

    private InputDevice rightController;
    private InputDevice leftController;

    [Header("Settings")]
    public int numberOfAnimations = 8;
    [Tooltip("空手状态下触发特殊动作的扳机阈值")]
    public float emptyHandTriggerThreshold = 0.7f; // 新增阈值参数

    void Start()
    {
        InitializeControllers();
    }

    void InitializeControllers()
    {
        // 更健壮的设备初始化（添加特征过滤）
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            rightHandDevices
        );
        if (rightHandDevices.Count > 0) rightController = rightHandDevices[0];

        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            leftHandDevices
        );
        if (leftHandDevices.Count > 0) leftController = leftHandDevices[0];
    }

    void Update()
    {
        if (!rightController.isValid || !leftController.isValid)
            InitializeControllers();

        HandleHandInput(
            rightController, rightHandAnimator,
            ref rightCurrentState, ref rightLastState,
            rightStateModels, true
        );

        HandleHandInput(
            leftController, leftHandAnimator,
            ref leftCurrentState, ref leftLastState,
            leftStateModels, false
        );
    }

    void HandleHandInput(InputDevice controller, Animator animator,
                        ref int currentState, ref int lastState,
                        StateModel[] stateModels, bool isRightHand)
    {
        if (!controller.isValid || animator == null) return;

        // ================ 修改点1：统一使用主按钮切换状态 ================
        if (GetButtonDown(controller, CommonUsages.primaryButton)) // 统一使用X/A键
        {
            currentState = (currentState + 1) % (numberOfAnimations + 1);
        }

        // ================ 原有抓握逻辑保持 ================
        controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        animator.SetBool("Hold", gripValue > 0.5f);

        // ================ 修改点2：增强扳机键逻辑 ================
        controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        animator.SetFloat("Trigger", triggerValue);

        // 空手状态特殊处理（假设状态0为空手）
        if (currentState == 0 && triggerValue > emptyHandTriggerThreshold)
        {
            animator.SetTrigger("PointAction"); // 需要Animator中添加PointAction Trigger参数
        }

        // ================ 原有状态更新逻辑 ================
        if (lastState != currentState)
        {
            lastState = currentState;
            animator.SetInteger("State", currentState);
            UpdateHandState(currentState, stateModels);
        }
    }

    bool GetButtonDown(InputDevice device, InputFeatureUsage<bool> button)
    {
        if (device.TryGetFeatureValue(button, out bool value))
            return value;
        return false;
    }

    void UpdateHandState(int stateNumber, StateModel[] models)
    {
        foreach (var item in models)
        {
            item.go.SetActive(item.stateNumber == stateNumber);
        }
    }
}