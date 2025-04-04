using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLaserController : MonoBehaviour
{
    [Header("XR Settings")]
    public XRRayInteractor rightRayInteractor; // 右手柄射线组件
    public XRRayInteractor leftRayInteractor;  // 左手柄射线组件

    [Header("Laser Settings")]
    [SerializeField] float maxDistance = 50f;
    [SerializeField] int maxBounces = 4;
    [SerializeField] float surfaceOffset = 0.01f;

    [Header("Interaction Settings")]
    private bool isInteractionBlocked = false;
    private float interactionBlockTimer = 0f;
    private const float interactionBlockDuration = 0.5f;

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
        UpdateLaserPath(); // 物理激光逻辑（仅用于特殊物体触发）
        HandleXRInput();   // 手柄交互逻辑（使用XR Ray Interactor）
        UpdateInteractionCooldown();
    }

    // 手柄交互逻辑（使用XR Ray Interactor）
    private void HandleXRInput()
    {
        if (isInteractionBlocked) return;

        // 检查右手柄交互
        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit rightHit))
        {
            HandleXRInteraction(rightHit, XRNode.RightHand);
        }

        // 检查左手柄交互
        if (leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit leftHit))
        {
            HandleXRInteraction(leftHit, XRNode.LeftHand);
        }
    }

    private void HandleXRInteraction(RaycastHit hit, XRNode handNode)
    {
        GameObject hitObject = hit.collider.gameObject;

        // 只处理镜子交互
        if (hitObject.CompareTag("mirror"))
        {
            bool isTriggerPressed = IsTriggerPressed(handNode);

            if (isTriggerPressed)
            {
                var mirror = hitObject.GetComponent<SimpleMirror>();
                if (mirror != null)
                {
                    mirror.Rotate();
                    isInteractionBlocked = true;
                    interactionBlockTimer = interactionBlockDuration;
                }
            }
        }
    }

    // 物理激光逻辑（添加Box Collider阻挡检测）
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

                    HandleLaserInteraction(hit);

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

    // 物理激光的交互处理（仅用于特殊Target）
    private void HandleLaserInteraction(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        if (hitObject.CompareTag("Target") && hitObject.layer == LayerMask.NameToLayer("Mirror"))
        {
            if (_lastHitMirror != hitObject)
            {
                Debug.Log("大吕，姑洗，夹钟，黄钟，仲吕");
                _lastHitMirror = hitObject;

                TextPopup textPopup = hitObject.GetComponent<TextPopup>();
                if (textPopup != null)
                {
                    textPopup.StartDisplay();
                }
            }
        }
    }

    // 冷却时间更新
    private void UpdateInteractionCooldown()
    {
        if (isInteractionBlocked)
        {
            interactionBlockTimer -= Time.deltaTime;
            if (interactionBlockTimer <= 0f)
            {
                isInteractionBlocked = false;
            }
        }
    }

    // 扳机键检测
    private bool IsTriggerPressed(XRNode handNode)
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(handNode, devices);
        if (devices.Count > 0)
        {
            if (devices[0].TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
            {
                return triggerValue;
            }
        }
        return false;
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