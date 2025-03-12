// BucketController.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BucketController : MonoBehaviour
{
    public Transform targetWell; // 第一个水井的Transform
    public GameObject waterObject; // 桶中的水
    public float pourAngle = 60f;
    public float activationDistance = 1.5f;

    private XRGrabInteractable _grabInteractable;
    private bool _isHeld;

    void Start()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(_ => _isHeld = true);
        _grabInteractable.selectExited.AddListener(_ => _isHeld = false);
    }

    void Update()
    {
        if (!_isHeld || !waterObject.activeSelf) return;

        // 距离检测
        float distance = Vector3.Distance(transform.position, targetWell.position);

        // 角度检测（使用局部坐标系的Z轴倾斜）
        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);

        if (distance <= activationDistance && tiltAngle >= pourAngle)
        {
            PerformPour();
        }
    }

    private void PerformPour()
    {
        waterObject.SetActive(false);
        GameManager.Instance.WaterPoured = true; // 更新全局状态
    }
}