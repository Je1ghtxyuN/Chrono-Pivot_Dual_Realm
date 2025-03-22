using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class DisappearOnCollision : MonoBehaviour
{
    [SerializeField] private string targetTag = "Target"; // 目标物体的标签

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    // 物理碰撞版本
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag) && grabInteractable.isSelected)
        {
            Destroy(gameObject);
        }
    }

    // 触发器版本（二选一）
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && grabInteractable.isSelected)
        {
            Destroy(gameObject);
            GameManager.SetThrowState(true);
        }
    }
}