using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;

[RequireComponent(typeof(Canvas))]
public class ExitButtonController : MonoBehaviour
{
    [Header("VRΩªª•…Ë÷√")]
    public XRRayInteractor rightRayInteractor;
    public XRRayInteractor leftRayInteractor;
    [Tooltip("…‰œﬂºÏ≤‚◊Ó¥Ûæ‡¿Î")]
    public float maxRaycastDistance = 100f;

    private Button button;

    private void Start()
    {
        button = GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("ÕÀ≥ˆ∞¥≈•Œ¥’“µΩ£°");
            return;
        }

        button.onClick.AddListener(OnExitClicked);
        SetRayInteractorMaxDistance();
    }

    private void SetRayInteractorMaxDistance()
    {
        if (rightRayInteractor != null) rightRayInteractor.maxRaycastDistance = maxRaycastDistance;
        if (leftRayInteractor != null) leftRayInteractor.maxRaycastDistance = maxRaycastDistance;
    }

    private void Update()
    {
        CheckVRInput();
    }

    private void CheckVRInput()
    {
        if (IsButtonTargeted() && (IsTriggerPressed(XRNode.RightHand) || IsTriggerPressed(XRNode.LeftHand)))
        {
            OnExitClicked();
        }
    }

    private bool IsButtonTargeted()
    {
        // ºÏ≤‚”“ ÷±˙
        if (rightRayInteractor != null && rightRayInteractor.TryGetCurrentUIRaycastResult(out var rightHit))
        {
            if (rightHit.gameObject == button.gameObject) return true;
        }

        // ºÏ≤‚◊Û ÷±˙
        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrentUIRaycastResult(out var leftHit))
        {
            if (leftHit.gameObject == button.gameObject) return true;
        }

        return false;
    }

    private bool IsTriggerPressed(XRNode handNode)
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(handNode, devices);
        if (devices.Count > 0)
        {
            return devices[0].TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue) && triggerValue;
        }
        return false;
    }

    private void OnExitClicked()
    {
        FindObjectOfType<VRBookController>()?.ExitBook();
    }
}