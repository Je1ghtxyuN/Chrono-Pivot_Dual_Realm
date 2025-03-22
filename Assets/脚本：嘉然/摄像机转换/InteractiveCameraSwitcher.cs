using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ObjectTeleportController : MonoBehaviour
{
    [Header("传送设置")]
    public Transform teleportAnchortrue; // 当pour为true时的目标点
    public Transform teleportAnchorfalse; // 当pour为false时的目标点
    public Transform xrRig;           // XR Rig父物体
    public float yOffset = 0.1f;      // 地面高度补偿

    private Vector3 originalPosition;  // 初始位置
    private Quaternion originalRotation; // 初始旋转
    private bool isLocked = false;     // 锁定状态
    private CharacterController characterController; // 角色控制器

    private Transform GetCurrentAnchor()//实际传送位置
    {
        return GameManager.pour ? teleportAnchortrue : teleportAnchorfalse;
    }

    void Start()
    {
        // 获取必要组件
        characterController = xrRig.GetComponent<CharacterController>();
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelect);

        // 记录初始位置（在Start而不是Awake确保XR系统已初始化）
        originalPosition = xrRig.position;
        originalRotation = xrRig.rotation;
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        if (!isLocked)
        {
            // 传送至目标点
            TeleportToAnchor();
            LockMovement(true);
        }
        else
        {
            // 返回原点并解锁
            ReturnToOriginalPosition();
            LockMovement(false);
        }
    }

    void TeleportToAnchor()
    {
        // 计算最终位置（包含高度补偿）
        Vector3 targetPos = GetCurrentAnchor().position + Vector3.up * yOffset;
        Quaternion targetRot = GetCurrentAnchor().rotation;

        // 移动XR Rig并同步角色控制器
        xrRig.SetPositionAndRotation(targetPos, targetRot);
        SyncCharacterController();

        isLocked = true;
    }

    void ReturnToOriginalPosition()
    {
        // 返回初始位置
        xrRig.SetPositionAndRotation(originalPosition, originalRotation);
        SyncCharacterController();

        isLocked = false;
    }

    void LockMovement(bool state)
    {
        // 冻结刚体物理模拟
        var rigidbody = xrRig.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = state;
            rigidbody.velocity = Vector3.zero;
        }
    }

    void SyncCharacterController()
    {
        // 同步角色控制器位置
        if (characterController)
        {
            characterController.enabled = false;
            characterController.transform.position = xrRig.position;
            characterController.enabled = true;
        }
    }
}
//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

//public class ObjectTeleportController : MonoBehaviour
//{
//    public Transform teleportAnchor; // 拖入传送目标空物体
//    public Transform xrRig;          // 拖入XR Rig父物体

//    void Start()
//    {
//        // 绑定交互事件
//        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
//        interactable.selectEntered.AddListener(OnSelect);
//    }

//    private void OnSelect(SelectEnterEventArgs args)
//    {
//        // 瞬间传送逻辑
//        Vector3 targetPosition = teleportAnchor.position;
//        Quaternion targetRotation = teleportAnchor.rotation;

//        // 移动整个XR Rig（包含摄像机）
//        xrRig.SetPositionAndRotation(targetPosition, targetRotation);
//    }
//}