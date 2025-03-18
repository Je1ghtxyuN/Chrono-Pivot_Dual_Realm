using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool pour = false; // 全局控制变量

    // 切换状态的方法
    public static void TogglePourState()
    {
        pour = !pour;
    }
}