using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jiguang : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxLength = 100f;
    // Start is called before the first frame update
    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 startPoint = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(startPoint, direction, out hit, maxLength))
        {
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, hit.point);
            // 检查击中的物体是否有 LaserReceiverAndEmitter 组件
            LaserReceiverAndEmitter receiver = hit.collider.GetComponent<LaserReceiverAndEmitter>();
            if (receiver != null)
            {
                receiver.ReceiveLaser();
            }
        }
        else
        {
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, startPoint + direction * maxLength);
        }
    }
}

public class LaserReceiverAndEmitter : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxLength = 100f;
    private bool isReceivingLaser = false;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (isReceivingLaser)
        {
            RaycastHit hit;
            Vector3 startPoint = transform.position;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(startPoint, direction, out hit, maxLength))
            {
                lineRenderer.SetPosition(0, startPoint);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(0, startPoint);
                lineRenderer.SetPosition(1, startPoint + direction * maxLength);
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public void ReceiveLaser()
    {
        isReceivingLaser = true;
        lineRenderer.enabled = true;
    }
}
