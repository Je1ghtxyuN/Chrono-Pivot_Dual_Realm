using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ActionBasedController), typeof(LineRenderer))]
public class MirrorRayInteraction : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField] private LayerMask mirrorLayer = 1 << 8; // 使用第8层Mirror
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float triggerThreshold = 0.2f;

    [Header("Feedback")]
    [SerializeField] private Material laserMaterial;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Cooldown")]
    [SerializeField] private float interactionCooldown = 0.5f;

    private ActionBasedController _controller;
    private LineRenderer _lineRenderer;
    private bool _isTriggerReady = true;
    private float _lastTriggerTime;

    void Awake()
    {
        _controller = GetComponent<ActionBasedController>();
        _lineRenderer = GetComponent<LineRenderer>();
        InitializeLaser();
    }

    void Update()
    {
        UpdateLaserState();
        ProcessTriggerInput();
    }

    private void InitializeLaser()
    {
        _lineRenderer.startWidth = 0.01f;
        _lineRenderer.endWidth = 0.005f;
        _lineRenderer.material = laserMaterial ? laserMaterial : new Material(Shader.Find("Unlit/Color"));
        _lineRenderer.material.color = inactiveColor;
    }

    private void UpdateLaserState()
    {
        bool hasHit = Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, maxDistance, mirrorLayer);

        if (hasHit)
        {
            DrawLaser(hit.point);
            UpdateLaserColor(hit);
        }
        else
        {
            DrawLaser(transform.position + transform.forward * maxDistance);
            _lineRenderer.material.color = inactiveColor;
        }
    }

    private void DrawLaser(Vector3 endPoint)
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, endPoint);
    }

    private void UpdateLaserColor(RaycastHit hit)
    {
        bool canInteract = hit.collider.GetComponent<SimpleMirror>() != null;
        _lineRenderer.material.color = canInteract ? activeColor : inactiveColor;
    }

    private void ProcessTriggerInput()
    {
        float triggerValue = _controller.activateAction.action.ReadValue<float>();

        if (triggerValue > triggerThreshold && _isTriggerReady)
        {
            TryInteract();
            _isTriggerReady = false;
            _lastTriggerTime = Time.time;
        }
        else if (triggerValue <= triggerThreshold && !_isTriggerReady)
        {
            _isTriggerReady = true;
        }
    }

    private void TryInteract()
    {
        if (Time.time - _lastTriggerTime < interactionCooldown) return;

        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, maxDistance, mirrorLayer))
        {
            var mirror = hit.collider.GetComponent<SimpleMirror>();
            if (mirror != null)
            {
                mirror.Rotate();
                _lastTriggerTime = Time.time;
                _controller.SendHapticImpulse(0.5f, 0.1f); // 触觉反馈
            }
        }
    }
}