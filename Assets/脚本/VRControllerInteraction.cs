// 挂载到手柄控制器上
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRControllerInteraction : MonoBehaviour
{
    [SerializeField] private XRController controller;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactionLayer;

    private InteractableObject currentTarget;

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(controller.transform.position, controller.transform.forward);

        if (Physics.Raycast(ray, out hit, maxDistance, interactionLayer))
        {
            InteractableObject newTarget = hit.collider.GetComponent<InteractableObject>();

            if (newTarget != currentTarget)
            {
                if (currentTarget != null) currentTarget.Highlight(false);
                currentTarget = newTarget;
                if (currentTarget != null) currentTarget.Highlight(true);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget.Highlight(false);
                currentTarget = null;
            }
        }
    }
}