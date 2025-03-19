using UnityEngine;

public class WellController : MonoBehaviour
{
    public Transform playerCamera; // 玩家的相机（头部）
    public float viewAngleThreshold = 0.8f; // 视角阈值，0.8约37度

    private bool isPlayerInRange = false;
    public static bool IsLookingAtWell { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            IsLookingAtWell = false;
        }
    }

    void Update()
    {
        if (isPlayerInRange)
        {
            Vector3 directionToWell = (transform.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, directionToWell);
            IsLookingAtWell = dot >= viewAngleThreshold;
        }
        else
            IsLookingAtWell = false;
    }
}