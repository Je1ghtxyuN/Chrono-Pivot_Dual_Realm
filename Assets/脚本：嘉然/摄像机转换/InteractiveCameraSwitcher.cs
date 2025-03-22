using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class InteractiveCameraSwitcher : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera cameraA;
    [SerializeField] private Camera cameraB;

    [Header("XR Settings")]
    [SerializeField] private XRRayInteractor rayInteractor;
    public PICOLeftJoystickMovement movementController;

    private bool isMainActive = true;

    private XRSimpleInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        InitializeCameras();
    }

    void Start()
    {
        interactable.selectEntered.AddListener(OnInteract);
    }

    private void InitializeCameras()
    {
        mainCamera.enabled = true;
        cameraA.enabled = false;
        cameraB.enabled = false;
        mainCamera.depth = 0;
        cameraA.depth = 1;
        cameraB.depth = 2;
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (args.interactorObject == rayInteractor)
        {
            ToggleCameraState();
            ToggleMovement();
        }
    }

    private void ToggleCameraState()
    {
        if (isMainActive)
        {
            mainCamera.enabled = false;
            cameraA.enabled = GameManager.pour;
            cameraB.enabled = !GameManager.pour;
        }
        else
        {
            mainCamera.enabled = true;
            cameraA.enabled = false;
            cameraB.enabled = false;
        }

        isMainActive = !isMainActive;
    }

    private void ToggleMovement()
    {
        bool canMove = mainCamera.enabled;
        if (!canMove)
        {
            movementController.enabled = false; // ½ûÓÃÒÆ¶¯Âß¼­
        }
        if (canMove)
        {
            movementController.enabled = true; // ÆôÓÃÒÆ¶¯Âß¼­
        }
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnInteract);
    }
}