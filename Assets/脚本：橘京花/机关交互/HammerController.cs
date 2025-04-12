using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider), typeof(XRGrabInteractable), typeof(Rigidbody))]
public class HammerController : MonoBehaviour
{
    [Header("调试设置")]
    public bool logColliderState = true;
    public float checkInterval = 1f;
    private float lastCheckTime;
    private Collider[] hammerColliders;

    [Header("强制设置")]
    public bool forceEnableColliders = true;

    private void Start()
    {
        // 获取所有碰撞体（包括子物体）
        hammerColliders = GetComponentsInChildren<Collider>();
        lastCheckTime = Time.time;

        Debug.Log($"锤子初始化完成，找到{hammerColliders.Length}个碰撞体");
    }

    private void Update()
    {
        // 定时检测
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckColliders();
            lastCheckTime = Time.time;
        }

        // 实时强制启用（如果启用该选项）
        if (forceEnableColliders)
        {
            ForceEnableColliders();
        }
    }

    private void CheckColliders()
    {
        if (!logColliderState) return;

        int disabledCount = 0;
        foreach (var collider in hammerColliders)
        {
            if (collider != null && !collider.enabled)
            {
                disabledCount++;
                Debug.LogWarning($"碰撞体被禁用: {collider.name} | 路径: {GetHierarchyPath(collider.transform)}", collider.gameObject);
            }
        }

        if (disabledCount > 0)
        {
            Debug.LogError($"发现{disabledCount}个被禁用的碰撞体！");
        }
        else if (logColliderState)
        {
            Debug.Log("所有碰撞体状态正常", gameObject);
        }
    }

    private void ForceEnableColliders()
    {
        foreach (var collider in hammerColliders)
        {
            if (collider != null && !collider.enabled)
            {
                collider.enabled = true;
                // Debug.Log($"已强制启用碰撞体: {collider.name}");
            }
        }
    }

    // 获取物体在层级中的完整路径
    private string GetHierarchyPath(Transform obj)
    {
        string path = obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }

    // 抓取/释放事件也做检测
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("锤子被抓取，开始碰撞体状态监控");
        ForceEnableColliders();
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("锤子被释放");
        ForceEnableColliders();
    }

    private void OnDestroy()
    {
        Debug.Log("锤子脚本销毁");
    }
}