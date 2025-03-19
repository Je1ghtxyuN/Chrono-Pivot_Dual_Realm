using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Diagnostics;

[RequireComponent(typeof(CharacterController))]
public class CameraTeleportSystem : MonoBehaviour
{
    [Header("Initial Position")]
    [Tooltip("The initial position where the player will spawn")]
    public Transform initialPosition;

    [Header("XR References")]
    public Transform xrOrigin;
    public Camera vrCamera;
    public XRRig xrRig;

    [Header("Teleport Positions")]
    public Transform positionWhenPourFalse;
    public Transform positionWhenPourTrue;

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;
    public LayerMask teleportLayer;

    private bool inTriggerZone;
    public static bool isTeleporting { get; private set; }
    private UnityEngine.UI.Image fadeImage;

    void Start()
    {
        InitializeComponents();
        CreateFadeCanvas();
        if (xrOrigin != null)
        {
            SetInitialPosition();
        }
        else
        {
            UnityEngine.Debug.LogError("XR components not initialized properly!");
        }
    }

    void SetInitialPosition()
    {
        if (initialPosition != null)
        {
            // Apply the same offset logic as teleportation
            Vector3 controllerOffset = xrRig.transform.position - xrOrigin.position;
            xrOrigin.position = initialPosition.position - controllerOffset;
            xrOrigin.rotation = initialPosition.rotation;
        }
    }

    void InitializeComponents()
    {
        if (xrOrigin == null)
            xrOrigin = GetComponent<Transform>();

        if (xrRig == null)
            xrRig = FindObjectOfType<XRRig>();

        if (vrCamera == null && xrRig != null)
            vrCamera = xrRig.GetComponentInChildren<Camera>();
    }

    void CreateFadeCanvas()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = vrCamera;
        canvas.planeDistance = 0.3f;

        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(canvas.transform, false);
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        fadeImage.color = Color.clear;

        canvas.gameObject.AddComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if (inTriggerZone && CheckConfirmInput() && !isTeleporting)
        {
            StartCoroutine(TeleportSequence());
        }
    }

    IEnumerator TeleportSequence()
    {
        isTeleporting = true;
        Transform targetPosition = GameManager.pour ? positionWhenPourTrue : positionWhenPourFalse;

        yield return StartCoroutine(FadeEffect(Color.black, fadeDuration));

        Vector3 controllerOffset = xrRig.transform.position - xrOrigin.position;
        xrOrigin.position = targetPosition.position - controllerOffset;
        xrOrigin.rotation = targetPosition.rotation;

        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(FadeEffect(Color.clear, fadeDuration));
        isTeleporting = false;
    }

    bool CheckConfirmInput()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, inputDevices);

        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed) && pressed)
                return true;
        }
        return Input.GetKeyDown(KeyCode.Space);
    }

    IEnumerator FadeEffect(Color targetColor, float duration)
    {
        float elapsed = 0;
        Color originalColor = fadeImage.color;

        while (elapsed < duration)
        {
            fadeImage.color = Color.Lerp(originalColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = targetColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((teleportLayer.value & (1 << other.gameObject.layer)) != 0)
            inTriggerZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if ((teleportLayer.value & (1 << other.gameObject.layer)) != 0)
            inTriggerZone = false;
    }
}