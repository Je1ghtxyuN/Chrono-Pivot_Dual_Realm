using UnityEngine;

public class CanvasFollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // 玩家摄像机的Transform
    public float distanceFromCamera = 2.0f; // Canvas与摄像机的距离

    void Update()
    {
        if (cameraTransform != null)
        {
            // 设置Canvas的位置在摄像机前方
            transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;

            // 设置Canvas的旋转与摄像机一致
            transform.rotation = cameraTransform.rotation;
        }
    }
}