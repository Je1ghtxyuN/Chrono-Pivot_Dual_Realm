// LaserPointerInteraction.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LaserPointerInteraction : MonoBehaviour
{
    [SerializeField] private XRController controller;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactionLayer;

    private InteractableObject currentTarget;

    void Update()
    {
        if (Physics.Raycast(new Ray(controller.transform.position, controller.transform.forward),
            out RaycastHit hit, maxDistance, interactionLayer))
        {
            HandleHitObject(hit);
        }
        else
        {
            ClearTarget();
        }
    }

    private void HandleHitObject(RaycastHit hit)
    {
        InteractableObject newTarget = hit.collider.GetComponent<InteractableObject>();

        if (newTarget == null)
        {
            ClearTarget();
            return;
        }

        if (newTarget != currentTarget)
        {
            currentTarget?.SetHighlight(false);
            currentTarget = newTarget;
            currentTarget.SetHighlight(true);
        }
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetHighlight(false);
            currentTarget = null;
        }
    }
}