using UnityEngine;

public class SINANcontroller: MonoBehaviour
{
    public Transform spoon;         // 勺子物体
    public Transform targetPosition; // 底座中心坐标
    public float moveSpeed = 5f;    // 移动平滑度
    public float rotateSpeed = 10f; // 旋转平滑度

    private bool isTriggered = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == spoon)
        {
            isTriggered = true;
            initialPosition = spoon.position;
            initialRotation = spoon.rotation;
            Debug.Log("SINANcontroller: OnTriggerEnter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == spoon)
        {
            isTriggered = false;
        }
    }

    void Update()
    {
        if (isTriggered)
        {
            // 位置插值
            spoon.position = Vector3.Lerp(
                spoon.position,
                targetPosition.position,
                Time.deltaTime * moveSpeed
            );

            // 旋转插值
            Quaternion targetRot = Quaternion.LookRotation(-Vector3.forward);
            spoon.rotation = Quaternion.Slerp(
                spoon.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed
            );
        }
    }
}