using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ObjectTeleportController : MonoBehaviour
{
    [Header("传送设置")]
    public Transform teleportAnchor;  // 目标传送点
    public Transform xrRig;           // XR Rig父物体
    public float yOffset = 0.1f;      // 地面高度补偿

    private Vector3 originalPosition;  // 初始位置
    private Quaternion originalRotation; // 初始旋转
    private bool isLocked = false;     // 锁定状态
    private CharacterController characterController; // 角色控制器

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
        Vector3 targetPos = teleportAnchor.position + Vector3.up * yOffset;
        Quaternion targetRot = teleportAnchor.rotation;

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
        // 同步角色控制器位置（解决碰撞问题）
        if (characterController)
        {
            characterController.enabled = false;
            characterController.transform.position = xrRig.position;
            characterController.enabled = true;
        }
    }

    // 可选：在编辑器可视化传送路径
    void OnDrawGizmosSelected()
    {
        if (teleportAnchor && xrRig)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, teleportAnchor.position);
            Gizmos.DrawWireSphere(teleportAnchor.position, 0.2f);
        }
    }
}