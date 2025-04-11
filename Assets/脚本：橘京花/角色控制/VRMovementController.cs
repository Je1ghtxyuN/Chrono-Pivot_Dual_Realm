using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class PICOLeftJoystickMovement : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 3f; // 移动速度
    [SerializeField] private float rotationSpeed = 120f; // 旋转速度
    [SerializeField] private float deadZone = 0.1f; // 摇杆死区

    [Header("XR参考")]
    [SerializeField] private XROrigin xrOrigin; // XR原点
    [SerializeField] private XRNode controllerNode = XRNode.LeftHand; // 控制器节点

    [Header("编辑器调试设置")]
    [SerializeField] private float mouseSensitivity = 2f; // 鼠标灵敏度
    [SerializeField] private bool invertMouseY = false; // 反转鼠标Y轴
    [SerializeField] private float keyboardMoveSpeed = 5f; // 键盘移动速度
    [SerializeField] private float keyboardRotationSpeed = 100f; // 键盘旋转速度

    private CharacterController characterController;
    private InputDevice targetDevice;
    private Vector2 joystickInput;
    private float xRotation = 0f; // 鼠标上下旋转角度
    private bool isEditorMode = false; // 是否编辑器模式

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // 检查是否在编辑器模式(无XR设备)
        var xrDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, xrDevices);
        isEditorMode = xrDevices.Count == 0 && Application.isEditor;

        if (!isEditorMode)
        {
            InitializeDevice(); // 初始化VR设备
        }
        else
        {
            // 设置鼠标状态(锁定并隐藏)
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    void InitializeDevice()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(controllerNode, devices);
        if (devices.Count > 0) targetDevice = devices[0];
    }

    void Update()
    {
        if (isEditorMode)
        {
            HandleEditorMovement(); // 处理编辑器移动
            HandleEditorLook(); // 处理编辑器视角
        }
        else
        {
            if (!targetDevice.isValid) InitializeDevice();

            if (xrOrigin != null && xrOrigin.Camera != null)
            {
                HandleVRMovement(); // 处理VR移动
            }
        }
    }

    // VR移动控制
    void HandleVRMovement()
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

    // 编辑器键盘移动控制
    void HandleEditorMovement()
    {
        // WASD移动
        float horizontal = -Input.GetAxis("Horizontal");
        float vertical = -Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            characterController.Move(moveDirection.normalized * keyboardMoveSpeed * Time.deltaTime);
        }

        // Q/E键旋转(鼠标控制的替代方案)
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -keyboardRotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, keyboardRotationSpeed * Time.deltaTime);
        }
    }

    // 编辑器鼠标视角控制
    void HandleEditorLook()
    {
        // 鼠标视角控制
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertMouseY ? 1 : -1);

        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制上下视角范围

        // 应用旋转到相机(子物体)
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            xrOrigin.Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // 备用方案(如果没有设置XROrigin)
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    // 计算移动方向
    Vector3 CalculateMovementDirection(Vector3 cameraForward, Vector3 cameraRight)
    {
        cameraForward.y = 0;
        cameraRight.y = 0;
        return (cameraForward.normalized * joystickInput.y + cameraRight.normalized * joystickInput.x).normalized;
    }

    // 判断是否在移动
    public bool IsMoving()
    {
        if (isEditorMode)
        {
            return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        }
        return joystickInput.magnitude >= deadZone;
    }

    void OnDestroy()
    {
        // 恢复鼠标状态
        if (isEditorMode)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}