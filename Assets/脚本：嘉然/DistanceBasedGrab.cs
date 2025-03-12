using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class DistanceBasedGrab : MonoBehaviour
{
    [SerializeField] private float _maxGrabDistance = 1.5f; // 最大抓取距离
    private XRGrabInteractable _grabInteractable;
    private Transform _controllerTransform;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnSelectEntered);
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    // 当物体被选中时记录控制器位置
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            _controllerTransform = rayInteractor.transform;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        _controllerTransform = null;
    }

    // 在 Update 中持续检测距离
    private void Update()
    {
        if (_controllerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _controllerTransform.position);
            if (distance > _maxGrabDistance)
            {
                // 强制释放抓取
                _grabInteractable.interactionManager.CancelInteractableSelection(_grabInteractable);
            }
        }
    }
}