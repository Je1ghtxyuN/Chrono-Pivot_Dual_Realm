using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class BeClocked : MonoBehaviour
{
    public bool isClocked = false;
    private ClockController clockController;

    [Header("交互设置")]
    public float cooldownTime = 3f;
    public float minHitVelocity = 0.3f;
    private float lastClockTime = -999f;

    [Header("锤子设置")]
    public string hammerTag = "Hammer";
    private Collider hammerCollider;
    private XRGrabInteractable grabInteractable;
    private Rigidbody hammerRigidbody;

    private void Start()
    {
        clockController = FindObjectOfType<ClockController>();
        if (clockController == null)
        {
            Debug.LogError("ClockController not found!");
        }

        InitializeHammer();
    }

    private void InitializeHammer()
    {
        GameObject hammer = GameObject.FindGameObjectWithTag(hammerTag);
        if (hammer == null)
        {
            Debug.LogError($"Hammer with tag '{hammerTag}' not found!");
            return;
        }

        hammerCollider = hammer.GetComponent<Collider>();
        grabInteractable = hammer.GetComponent<XRGrabInteractable>();
        hammerRigidbody = hammer.GetComponent<Rigidbody>();

        if (hammerCollider == null) Debug.LogError("Hammer collider missing!");
        if (grabInteractable == null) Debug.LogError("XRGrabInteractable component missing!");

        // 强制配置抓取交互设置
        ConfigureGrabInteractable();
    }

    private void ConfigureGrabInteractable()
    {
        // 确保有刚体组件
        if (hammerRigidbody == null)
        {
            hammerRigidbody = grabInteractable.gameObject.AddComponent<Rigidbody>();
        }

        // 关键物理设置
        hammerRigidbody.isKinematic = false;
        hammerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // 强制启用所有碰撞体
        foreach (var collider in grabInteractable.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }

        // 事件监听
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 抓取时确保碰撞体激活
        foreach (var collider in grabInteractable.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }

        // 确保物理参数正确
        hammerRigidbody.isKinematic = false;
        hammerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // 释放时不需要特殊处理
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsValidHit(collision))
        {
            Clock(collision.relativeVelocity);
        }
    }

    private bool IsValidHit(Collision collision)
    {
        return collision.collider == hammerCollider &&
               Time.time - lastClockTime >= cooldownTime &&
               collision.relativeVelocity.magnitude >= minHitVelocity;
    }

    public void Clock(Vector3 hitVelocity)
    {
        lastClockTime = Time.time;
        isClocked = true;
        Debug.Log($"编钟被敲击: {gameObject.name}, 速度: {hitVelocity.magnitude:F1}m/s");

        clockController?.PlayOnce(this);
        Invoke(nameof(ResetClockState), 0.1f);
    }

    private void ResetClockState()
    {
        isClocked = false;
    }

    private void OnGUI()
    {
        float remainingCooldown = Mathf.Max(0, cooldownTime - (Time.time - lastClockTime));
        if (remainingCooldown > 0)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 30),
                     $"冷却: {remainingCooldown:F1}s");
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}