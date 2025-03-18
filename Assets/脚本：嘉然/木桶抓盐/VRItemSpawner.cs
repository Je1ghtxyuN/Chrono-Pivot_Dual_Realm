using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class VRItemSpawner : MonoBehaviour
{
    [Header("VR Settings")]
    public Transform targetObject;           // 需要注视的目标物体
    public Transform handController;         // 生成物品的手部控制器

    [Header("Detection Settings")]
    [SerializeField] private float activationRadius = 2f;
    [SerializeField][Range(0.5f, 1f)] private float faceThreshold = 0.8f;

    [Header("Prefab Reference")]
    public GameObject spawnPrefab;           // 要生成的预制体

    private bool isPlayerInRange = false;
    private Transform playerCamera;
    private GameObject spawnedItem;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (CheckAllConditions())
        {
            SpawnItemInHand();
        }
    }

    private bool CheckAllConditions()
    {
        return isPlayerInRange &&
               CheckFaceTarget() &&
               CheckTriggerInput();
    }

    private bool CheckFaceTarget()
    {
        Vector3 directionToTarget = (targetObject.position - playerCamera.position).normalized;
        float dotProduct = Vector3.Dot(playerCamera.forward, directionToTarget);
        return dotProduct >= faceThreshold;
    }

    private bool CheckTriggerInput()
    {
        return Input.GetButtonDown("XRI_Right_Trigger");
    }

    private void SpawnItemInHand()
    {
        if (spawnedItem == null)
        {
            spawnedItem = Instantiate(spawnPrefab, handController.position, handController.rotation);
            spawnedItem.transform.SetParent(handController);
            AdjustItemPosition(spawnedItem.transform);
        }
    }

    private void AdjustItemPosition(Transform item)
    {
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
    }

    private void ConfigureCollider()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = activationRadius;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}