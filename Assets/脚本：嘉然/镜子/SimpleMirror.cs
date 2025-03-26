using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleMirror : MonoBehaviour
{
    [SerializeField] float rotateAngle = 45f;
    public bool isRotated = false;
    private Vector3 originalRotation;

    void Start()
    {
        originalRotation = transform.eulerAngles;
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Rotate());
    }

    public void Rotate()
    {
        float targetY = isRotated ? originalRotation.y : originalRotation.y + rotateAngle;
        transform.eulerAngles = new Vector3(
            originalRotation.x,
            targetY,
            originalRotation.z
        );
        isRotated = !isRotated;
    }
}