using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // 绑定的射线交互器
    public InputActionReference submitAction; // 确定键输入（如Trigger）
    public Transform playerCamera; // 玩家的头部或手部视角

    private XRBaseInteractable currentInteractable;
    private bool isHoldingObject = false;

    void Start()
    {
        // 监听抓取和释放事件
        rayInteractor.selectEntered.AddListener(OnSelectEntered);
        rayInteractor.selectExited.AddListener(OnSelectExited);
    }

    void Update()
    {
        if (isHoldingObject && submitAction.action.WasPressedThisFrame())
        {
            // 检测是否满足水井条件
            if (WellController.IsLookingAtWell)
            {
                // 销毁物体并更新全局变量
                Destroy(currentInteractable.gameObject);
                GameManager.SetThrowState(true);
                currentInteractable = null;
                isHoldingObject = false;
            }
        }
    }

    // 抓取物体时触发
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractable = args.interactableObject as XRBaseInteractable;
        isHoldingObject = true;
    }

    // 释放物体时触发
    private void OnSelectExited(SelectExitEventArgs args)
    {
        currentInteractable = null;
        isHoldingObject = false;
    }
}