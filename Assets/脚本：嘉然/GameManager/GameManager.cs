using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool pour = false; // 全局控制变量
    private static bool _throw;

    public static void SetThrowState(bool newState)
    {
        _throw = newState;
    }
    // 切换状态的方法
    public static void TogglePourState()
    {
        pour = !pour;
    }
}