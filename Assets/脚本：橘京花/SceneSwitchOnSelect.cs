//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.SceneManagement;

//public class SceneSwitchOnSelect : MonoBehaviour
//{
//    public Transform player; // 玩家的位置（通常是 XR Origin 或 Camera）
//    public float interactionDistance = 2.0f; // 交互的最大距离
//    public string targetSceneName = "01"; // 要切换的目标场景名称

//    private XRBaseInteractable targetInteractable; // 目标物体
//    private bool isHighlighted = false; // 物体是否被高亮

//    void Update()
//    {
//        // 检查玩家与物体的距离
//        if (targetInteractable != null && IsPlayerNearObject(targetInteractable.transform))
//        {
//            // 检查物体是否被射线照射并高亮
//            if (isHighlighted)
//            {
//                // 检查是否按下了 select 键
//                if (Input.GetButtonDown("XRI_Right_Select") || Input.GetButtonDown("XRI_Left_Select"))
//                {
//                    // 切换场景
//                    SceneManager.LoadScene(targetSceneName);
//                }
//            }
//        }
//    }

//    // 判断玩家是否在物体附近
//    private bool IsPlayerNearObject(Transform objectTransform)
//    {
//        float distance = Vector3.Distance(player.position, objectTransform.position);
//        return distance <= interactionDistance;
//    }

//    // 当物体被射线照射并高亮时调用
//    public void OnHighlightEntered(XRBaseInteractable interactable)
//    {
//        if (interactable != null)
//        {
//            targetInteractable = interactable;
//            isHighlighted = true;
//        }
//    }

//    // 当物体不再被射线照射并高亮时调用
//    public void OnHighlightExited(XRBaseInteractable interactable)
//    {
//        if (interactable == targetInteractable)
//        {
//            targetInteractable = null;
//            isHighlighted = false;
//        }
//    }
//}