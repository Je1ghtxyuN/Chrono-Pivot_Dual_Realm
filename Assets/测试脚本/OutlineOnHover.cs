using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Outline))]
public class OutlineOnHover : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    // 当悬停开始
    public void EnableOutline(HoverEnterEventArgs args)
    {
        outline.enabled = true;
    }

    // 当悬停结束
    public void DisableOutline(HoverExitEventArgs args)
    {
        outline.enabled = false;
    }
}