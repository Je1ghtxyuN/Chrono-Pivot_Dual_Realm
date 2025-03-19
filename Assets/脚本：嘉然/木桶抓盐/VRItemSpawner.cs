using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class VRItemSpawner : MonoBehaviour
{
    [Header("XR References")]
    [Tooltip("如果未手动指定，将自动查找右手控制器")]
    [SerializeField] private XRController rightHandController; // 添加SerializeField支持手动拖拽
    public Transform handController;
    public Transform targetObject;

    [Header("Spawn Settings")]
    public GameObject spawnPrefab;
    public LayerMask spawnLayer;
    [Range(0.5f, 1f)] public float faceThreshold = 0.8f;
    public float activationRadius = 2f;

    private Transform playerCamera;
    private GameObject spawnedItem;
    private SphereCollider detectionCollider;

    void Awake()
    {
        // 初始化检测碰撞体
        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.radius = activationRadius;
        detectionCollider.isTrigger = true;

        // 确保控制器初始化在Awake阶段完成
        InitializeControllers();
    }

    void InitializeControllers()
    {
        // 优先使用手动拖入的控制器
        if (rightHandController == null)
        {
            // 按名称查找控制器
            rightHandController = GameObject.Find("RightHand Controller")?.GetComponent<XRController>();

            // 备用方案：遍历查找
            if (rightHandController == null)
            {
                foreach (var controller in FindObjectsOfType<XRController>())
                {
                    if (controller.inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                    {
                        rightHandController = controller;
                        break;
                    }
                }
            }
        }

        // 自动绑定手部变换组件
        if (rightHandController != null && handController == null)
        {
            handController = rightHandController.transform;
        }
    }

    void Start()
    {
        playerCamera = Camera.main.transform;
        FinalCheckControllers();
    }

    void FinalCheckControllers()
    {
        if (rightHandController == null)
        {
            enabled = false; // 禁用脚本防止错误
        }
    }

    void Update()
    {
        if (CanSpawnItem())
            SpawnItemInHand();
    }

    bool CanSpawnItem()
    {
        return rightHandController != null &&
               !CameraTeleportSystem.isTeleporting &&
               IsPlayerInRange() &&
               IsFacingTarget() &&
               CheckTriggerInput();
    }

    bool IsPlayerInRange()
    {
        return (spawnLayer.value & (1 << gameObject.layer)) != 0;
    }

    bool IsFacingTarget()
    {
        Vector3 direction = (targetObject.position - playerCamera.position).normalized;
        return Vector3.Dot(playerCamera.forward, direction) >= faceThreshold;
    }

    bool CheckTriggerInput()
    {
        if (rightHandController.inputDevice.TryGetFeatureValue(
            CommonUsages.triggerButton,
            out bool triggerPressed))
        {
            return triggerPressed;
        }
        return false;
    }

    void SpawnItemInHand()
    {
        if (spawnedItem == null && handController != null)
        {
            spawnedItem = Instantiate(spawnPrefab, handController.position, handController.rotation);
            spawnedItem.transform.SetParent(handController);

            if (spawnedItem.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, spawnLayer))
        {
            gameObject.layer = LayerMaskToLayer(spawnLayer);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, spawnLayer))
        {
            gameObject.layer = 0;
        }
    }

    // 工具方法：将LayerMask转换为具体层级
    int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 0)
        {
            layer >>= 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}