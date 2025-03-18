using UnityEngine;
using UnityEngine.XR;

public class CustomVRGrab : MonoBehaviour
{
    [Header("抓取参数")]
    public float grabDistance = 0.2f;    // 抓取检测距离
    public Transform grabPoint;          // 抓取锚点
    public float throwForce = 1.5f;      // 投掷力度

    private GameObject grabbedObject;
    private bool isGrabbing;
    private FixedJoint grabJoint;

    void Update()
    {
        // 检测手柄输入（以右手为例）
        var device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed);

        // 抓取逻辑
        if (triggerPressed && !isGrabbing)
        {
            TryGrab();
        }
        else if (!triggerPressed && isGrabbing)
        {
            ReleaseObject();
        }

        // 物体跟随
        if (isGrabbing && grabbedObject != null)
        {
            grabbedObject.GetComponent<Rigidbody>().velocity =
                (grabPoint.position - grabbedObject.transform.position) * 10f;
        }
    }

    void TryGrab()
    {
        // 射线检测抓取物体
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                grabbedObject = hit.collider.gameObject;
                GrabObject();
            }
        }
    }

    void GrabObject()
    {
        // 创建物理连接
        grabJoint = gameObject.AddComponent<FixedJoint>();
        grabJoint.connectedBody = grabbedObject.GetComponent<Rigidbody>();

        // 禁用重力
        grabbedObject.GetComponent<Rigidbody>().useGravity = false;

        isGrabbing = true;
    }

    void ReleaseObject()
    {
        // 销毁连接
        Destroy(grabJoint);

        // 恢复物理属性
        if (grabbedObject != null)
        {
            var rb = grabbedObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.velocity = transform.TransformDirection(Vector3.forward) * throwForce;
        }

        isGrabbing = false;
        grabbedObject = null;
    }
}