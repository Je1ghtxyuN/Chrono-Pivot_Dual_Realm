using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLaserController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxDistance = 50f;
    [SerializeField] int maxBounces = 6;        // 最大弹射次数
    [SerializeField] float surfaceOffset = 0.01f; // 表面偏移量

    [Header("Debug")]
    [SerializeField] bool showGizmos = true;

    private LineRenderer _line;
    private LayerMask _reflectionMask;
    private GameObject _lastHitMirror;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _reflectionMask = LayerMask.GetMask("Mirror");
    }

    void Update()
    {
        UpdateLaserPath();
    }

    private void UpdateLaserPath()
    {
        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);

        Vector3 currentDir = transform.forward;
        Vector3 currentPos = transform.position;
        int bounceCount = 0;
        _lastHitMirror = null;

        while (bounceCount <= maxBounces) // 包含初始发射
        {
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, maxDistance, _reflectionMask))
            {
                // 添加碰撞点
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, hit.point);

                // 处理镜子交互
                HandleMirrorInteraction(hit);

                // 计算反射方向
                currentDir = Vector3.Reflect(currentDir, hit.normal);
                currentPos = hit.point + currentDir * surfaceOffset;
                bounceCount++;
            }
            else
            {
                // 添加终点
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, currentPos + currentDir * maxDistance);
                break;
            }
        }
    }

    private void HandleMirrorInteraction(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Mirror"))
        {
            if (hit.collider.gameObject != _lastHitMirror)
            {
                var mirror = hit.collider.GetComponent<SimpleMirror>();
                if (mirror != null)
                {
                    mirror.Rotate();
                    _lastHitMirror = hit.collider.gameObject;
                }
            }
        }
        else
        {
            _lastHitMirror = null;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
    }
#endif
}