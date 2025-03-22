using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SpawnPrefabOnGrab : MonoBehaviour
{
    [Header("…˙≥…≈‰÷√")]
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private Transform attachTransform;

    private XRSimpleInteractable simpleInteractable;
    private GameObject spawnedInstance;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnGrabStart);
        simpleInteractable.selectExited.AddListener(OnGrabEnd);
    }

    private void OnGrabStart(SelectEnterEventArgs args)
    {
        spawnedInstance = Instantiate(handPrefab);
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;

        if (attachTransform != null)
            spawnedInstance.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        else
            spawnedInstance.transform.SetParent(interactor.transform, false);

        XRGrabInteractable newGrab = spawnedInstance.AddComponent<XRGrabInteractable>();
        interactor.StartManualInteraction(newGrab);
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        if (spawnedInstance != null && !spawnedInstance.GetComponent<XRGrabInteractable>().isSelected)
            Destroy(spawnedInstance);
    }

    private void OnDestroy()
    {
        simpleInteractable.selectEntered.RemoveListener(OnGrabStart);
        simpleInteractable.selectExited.RemoveListener(OnGrabEnd);
    }
}