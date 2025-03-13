using UnityEngine;

public class BridgeController : MonoBehaviour
{
    public Transform bridge;         
    public Transform arrow;
   
    public float rotateSpeed = 10f; // 旋转平滑度

    private bool isTriggered = false;
    private Quaternion initialRotation;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == arrow)
        {
            isTriggered = true;
            
            initialRotation = bridge.rotation;
            Debug.Log("Enter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == bridge)
        {
            isTriggered = false;
        }
    }

    void Update()
    {
        if (isTriggered)
        {
            // 旋转插值
            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward);
            bridge.rotation = Quaternion.Slerp(
                bridge.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed
            );
        }
    }
}