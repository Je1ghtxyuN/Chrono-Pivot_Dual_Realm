using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpawnPrefabOnGrab : MonoBehaviour
{
    [Header("生成配置")]
    [SerializeField] private GameObject handPrefab; // 需出现在手上的预制体
    [SerializeField] private Transform attachTransform; // 抓取点

    private XRGrabInteractable grabInteractable;
    private GameObject spawnedInstance;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);
    }

    private void OnGrabStart(SelectEnterEventArgs args)
    {
        // 实例化预制体并附加到交互器
        spawnedInstance = Instantiate(handPrefab);
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;

        // 设置抓取点（若未指定则使用默认位置）
        if (attachTransform != null)
            spawnedInstance.transform.SetPositionAndRotation(
                attachTransform.position,
                attachTransform.rotation
            );
        else
            spawnedInstance.transform.SetParent(interactor.transform, false);

        // 强制抓取新物体
        XRGrabInteractable newGrab = spawnedInstance.AddComponent<XRGrabInteractable>();
        interactor.StartManualInteraction(newGrab);
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        if (spawnedInstance != null)
            Destroy(spawnedInstance);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabStart);
        grabInteractable.selectExited.RemoveListener(OnGrabEnd);
    }
}