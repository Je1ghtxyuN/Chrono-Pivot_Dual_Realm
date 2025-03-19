using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEndGame : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // 引用XR射线交互器

    void Update()
    {
        // 检测扳机键是否按下
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            // 检测射线是否对准UI按钮
            if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
            {
                Button button = raycastResult.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    // 退出游戏
                    EndGame();
                }
            }
        }
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