//using Unity.XR.CoreUtils;
//using UnityEngine;
//using UnityEngine.XR;

//[RequireComponent(typeof(CharacterController))]
//public class VRMMovementControllerSimplified : MonoBehaviour
//{
//    [Header("Movement Settings")]
//    [SerializeField] private float moveSpeed = 3f;
//    [SerializeField] private float rotationSpeed = 120f;
//    [SerializeField] private float deadZone = 0.1f;

//    [Header("XR References")]
//    [SerializeField] private XROrigin xrOrigin;

//    private CharacterController characterController;

//    void Start()
//    {
//        characterController = GetComponent<CharacterController>();
//    }

//    void Update()
//    {
//        if (xrOrigin != null && xrOrigin.Camera != null)
//        {
//            HandleMovement();
//        }
//    }

//    void HandleMovement()
//    {
//        // 获取摇杆输入
//        float horizontal = Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal");
//        float vertical = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical");

//        // 检查死区
//        if (new Vector2(horizontal, vertical).magnitude < deadZone) return;

//        // 计算移动方向（基于相机的局部坐标）
//        Vector3 moveDirection = CalculateMovementDirection(
//            xrOrigin.Camera.transform.forward,
//            xrOrigin.Camera.transform.right,
//            horizontal,
//            vertical
//        );

//        // 移动角色
//        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

//        // 旋转角色（基于摇杆的水平输入）
//        if (horizontal != 0)
//        {
//            xrOrigin.RotateAroundCameraPosition(
//                Vector3.up,
//                horizontal * rotationSpeed * Time.deltaTime
//            );
//        }
//    }

//    Vector3 CalculateMovementDirection(Vector3 cameraForward, Vector3 cameraRight, float horizontal, float vertical)
//    {
//        // 忽略相机的垂直分量（防止角色向上或向下移动）
//        cameraForward.y = 0;
//        cameraRight.y = 0;

//        // 计算移动方向
//        return (cameraForward.normalized * vertical + cameraRight.normalized * horizontal).normalized;
//    }
//}