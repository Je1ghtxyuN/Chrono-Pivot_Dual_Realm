using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLaserController : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] float maxDistance = 50f;
    [SerializeField] int maxBounces = 4;
    [SerializeField] float surfaceOffset = 0.01f;

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

        while (bounceCount <= maxBounces)
        {
            // 检测所有碰撞（包括Box Collider）
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, maxDistance))
            {
                // 检查是否碰到Box Collider（非Mirror层）
                if (hit.collider is BoxCollider && !LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Mirror"))
                {
                    // 如果碰到Box Collider，激光在此终止
                    _line.positionCount++;
                    _line.SetPosition(_line.positionCount - 1, hit.point);
                    break;
                }

                // 只处理Mirror层的反射
                if (((1 << hit.collider.gameObject.layer) & _reflectionMask) != 0)
                {
                    _line.positionCount++;
                    _line.SetPosition(_line.positionCount - 1, hit.point);

                    HandleMirrorInteraction(hit);

                    currentDir = Vector3.Reflect(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * surfaceOffset;
                    bounceCount++;
                }
                else
                {
                    // 碰到非反射物体，激光终止
                    _line.positionCount++;
                    _line.SetPosition(_line.positionCount - 1, hit.point);
                    break;
                }
            }
            else
            {
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, currentPos + currentDir * maxDistance);
                break;
            }
        }
    }

    private void HandleMirrorInteraction(RaycastHit hit)

    {
        GameObject hitObject = hit.collider.gameObject;

        if (hitObject.CompareTag("Target") &&
            hitObject.layer == LayerMask.NameToLayer("Mirror"))
        {
            if (_lastHitMirror != hitObject)
            {
                _lastHitMirror = hitObject;
                TextPopup textPopup = hitObject.GetComponent<TextPopup>();
                if (textPopup != null)
                {
                    textPopup.StartDisplay();
                }
            }
            return;
        }
        if (hitObject.CompareTag("Mirror"))
        {
            if (_lastHitMirror != hitObject)
            {
                var mirror = hitObject.GetComponent<SimpleMirror>();
                if (mirror != null)
                {
                    _lastHitMirror = hitObject;
                }
            }
        }
        else
        {
            _lastHitMirror = null;
        }
    }
}