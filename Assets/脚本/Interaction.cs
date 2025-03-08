using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Interaction : MonoBehaviour
{
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField][Range(1, 10)] private float outlineWidth = 5f;
    [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineAll;

    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();

        outline.OutlineMode = outlineMode;
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;

        // 对复杂模型启用预计算
        if (GetComponent<MeshFilter>()?.sharedMesh.vertexCount > 5000)
            outline.PrecomputeOutline = true;
    }

    public void Highlight(bool state)
    {
        if (outline != null)
            outline.enabled = state;
    }
}