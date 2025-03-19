using UnityEngine;

public class TimeTravelTrigger : MonoBehaviour
{
    // 引用 TimetravelController 脚本
    public TimetravelController timeTravelController;

    // 当其他碰撞器进入触发器时调用
    private void OnTriggerEnter(Collider other)
    {
        // 检查触发碰撞的对象是否是玩家
        if (other.CompareTag("Player"))
        {
            // 调用 TimetravelController 中的 Timetravel 方法
            if (timeTravelController != null)
            {
                timeTravelController.Timetravel();
            }
            else
            {
                Debug.LogWarning("TimetravelController 未分配！");
            }
        }
    }
}