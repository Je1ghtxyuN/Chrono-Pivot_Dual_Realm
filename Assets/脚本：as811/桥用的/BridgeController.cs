using UnityEngine;


public class BridgeController : MonoBehaviour
{
    [Header("基础设置")]
    public Transform bridge;
    public Transform arrow;
    public float rotateSpeed = 10f; // 旋转平滑度
    [Header("粒子系统设置")]
    public ParticleController particleController;
    //需要改旋转的目标角度时改这里
    private Quaternion targetRot = Quaternion.LookRotation(Vector3.forward);
    
    
    private bool isTriggered = false;
    private Quaternion initialRotation;
    //保护性，防止多次触发
    private bool defend = false;
    void Start()
    {
        particleController=GetComponent<ParticleController>();
        if (bridge != null)
        {
            initialRotation = bridge.rotation;
        }
        else
        {
            Debug.LogError("哥们你桥呢？", this);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform == arrow)
        {
            isTriggered = true;
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
        if (bridge.rotation == targetRot && !defend)
        {
            StartCoroutine(particleController.PlayAndStop());
            defend = true;
        }
        if (isTriggered)
        {
            // 旋转插值
            
            bridge.rotation = Quaternion.Slerp(
                bridge.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed
            );
            
        }

    }
}