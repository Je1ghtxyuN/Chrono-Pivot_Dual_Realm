// InteractableObject.cs
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableObject : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        // 大型模型优化方案：
        // 1. 在编辑器中手动启用Precompute Outline
        // 2. 添加LOD组优化渲染
    }

    public void SetHighlight(bool isHighlighted)
    {
        outline.enabled = isHighlighted;
    }
}