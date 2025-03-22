using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class PICOLeftJoystickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadZone = 0.1f;

    [Header("XR References")]
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private XRNode controllerNode = XRNode.LeftHand;

    private CharacterController characterController;
    private InputDevice targetDevice;
    private Vector2 joystickInput;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        InitializeDevice();
    }

    void InitializeDevice()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(controllerNode, devices);
        if (devices.Count > 0) targetDevice = devices[0];
    }

    void Update()
    {
        if (!targetDevice.isValid) InitializeDevice();

        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickInput))
        {
            if (joystickInput.magnitude < deadZone) return;

            // 使用XROrigin的相机方向计算移动
            Vector3 moveDirection = CalculateMovementDirection(
                xrOrigin.Camera.transform.forward,
                xrOrigin.Camera.transform.right
            );

            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 使用XROrigin的旋转方法保持一致性
            if (joystickInput.x != 0)
            {
                xrOrigin.RotateAroundCameraPosition(
                    Vector3.up,
                    joystickInput.x * rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    Vector3 CalculateMovementDirection(Vector3 cameraForward, Vector3 cameraRight)
    {
        cameraForward.y = 0;
        cameraRight.y = 0;
        return (cameraForward.normalized * joystickInput.y + cameraRight.normalized * joystickInput.x).normalized;
    }

    // 判断玩家是否在移动
    public bool IsMoving()
    {
        return joystickInput.magnitude >= deadZone;
    }
}