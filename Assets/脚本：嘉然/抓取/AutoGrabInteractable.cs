using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))] // 强制要求刚体组件
public class AutoGrabInteractable : MonoBehaviour
{
    private void Awake()
    {
        // 自动添加必要组件
        if (!TryGetComponent<XRGrabInteractable>(out _))
        {
            var grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            ConfigureGrabParameters(grabInteractable);
        }

        EnsurePhysicsComponents();
    }

    private void ConfigureGrabParameters(XRGrabInteractable interactable)
    {
        // 设置抓取参数
        interactable.movementType = XRBaseInteractable.MovementType.Instantaneous;
        interactable.throwVelocityScale = 1.5f;
        interactable.interactionLayers = InteractionLayerMask.GetMask("Default"); // 需与Interactor层级匹配
    }

    private void EnsurePhysicsComponents()
    {
        // 确保存在碰撞体和刚体
        if (!TryGetComponent<Collider>(out _))
        {
            gameObject.AddComponent<BoxCollider>();
        }

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }
}