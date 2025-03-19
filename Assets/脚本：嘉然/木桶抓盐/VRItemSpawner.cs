using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class VRItemSpawner : MonoBehaviour
{
    [Header("XR References")]
    public XRController rightHandController;
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
        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.radius = activationRadius;
        detectionCollider.isTrigger = true;
    }

    void Start()
    {
        playerCamera = Camera.main.transform;
        if (rightHandController == null)
            rightHandController = FindObjectOfType<XRController>();
    }

    void Update()
    {
        if (CanSpawnItem())
            SpawnItemInHand();
    }

    bool CanSpawnItem()
    {
        return !CameraTeleportSystem.isTeleporting &&
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
        rightHandController.inputDevice.TryGetFeatureValue(
            CommonUsages.triggerButton,
            out bool triggerPressed);
        return triggerPressed;
    }

    void SpawnItemInHand()
    {
        if (spawnedItem == null)
        {
            spawnedItem = Instantiate(spawnPrefab);
            spawnedItem.transform.SetPositionAndRotation(
                handController.position,
                handController.rotation
            );

            if (spawnedItem.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;

            spawnedItem.transform.SetParent(handController);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((spawnLayer.value & (1 << other.gameObject.layer)) != 0)
            gameObject.layer = spawnLayer;
    }

    void OnTriggerExit(Collider other)
    {
        if ((spawnLayer.value & (1 << other.gameObject.layer)) != 0)
            gameObject.layer = 0;
    }
}