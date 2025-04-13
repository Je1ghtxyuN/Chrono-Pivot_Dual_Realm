using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SimpleMirror : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] float rotateAngle = 45f;
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float cooldown = 0.3f;

    private bool isRotated = false;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private float lastRotateTime;

    void Start()
    {
        originalRotation = transform.rotation;
        targetRotation = originalRotation;
        GetComponent<XRSimpleInteractable>().activated.AddListener(_ => Rotate());
    }

    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void Rotate()
    {
        if (Time.time - lastRotateTime < cooldown) return;
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) return;

        RotateLogic();
    }

    private void RotateLogic()
    {
        float targetY = isRotated ?
            originalRotation.eulerAngles.y :
            originalRotation.eulerAngles.y + rotateAngle;

        targetRotation = Quaternion.Euler(
            originalRotation.eulerAngles.x,
            targetY,
            originalRotation.eulerAngles.z
        );

        isRotated = !isRotated;
        lastRotateTime = Time.time;
    }
}