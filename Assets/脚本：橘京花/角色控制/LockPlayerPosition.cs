using UnityEngine;

public class LockPlayerPosition : MonoBehaviour
{
    // 玩家需要固定的位置
    public Vector3 fixedPosition = new Vector3(0, 0, 0);

    void Update()
    {
        // 每帧将玩家的位置重置为固定位置
        transform.position = fixedPosition;
    }
}