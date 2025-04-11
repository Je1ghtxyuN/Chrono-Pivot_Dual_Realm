using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIEndGame : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // 引用XR射线交互器

    void Update()
    {
        // VR控制器扳机键交互
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            CheckAndExecuteButtonClick();
        }

        // 鼠标左键交互（用于调试）
        if (Input.GetMouseButtonDown(0))
        {
            CheckAndExecuteButtonClick();
        }
    }

    void CheckAndExecuteButtonClick()
    {
        // 检测射线是否对准UI按钮（VR和鼠标都适用）
        if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            Button button = raycastResult.gameObject.GetComponent<Button>();
            if (button != null)
            {
                // 退出游戏
                EndGame();
            }
        }

        // 额外检测鼠标点击（确保在编辑器中也能工作）
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    EndGame();
                    break;
                }
            }
        }
#endif
    }

    void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止播放
#else
        Application.Quit(); // 在构建版本中退出游戏
#endif
    }
}