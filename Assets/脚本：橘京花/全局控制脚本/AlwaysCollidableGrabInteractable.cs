using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AlwaysCollidableGrabInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        ForceEnableColliders();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        ForceEnableColliders();
        base.OnSelectExited(args);
    }

    private void ForceEnableColliders()
    {
        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    private void Update()
    {
        // ³ÖÐø¼à¿Ø
        if (isSelected)
        {
            ForceEnableColliders();
        }
    }
}