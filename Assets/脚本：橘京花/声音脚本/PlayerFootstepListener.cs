using UnityEngine;

public class PlayerFootstepListener : MonoBehaviour
{
    public FootstepAudioData FootstepAudioData;
    public AudioSource FootstepAudioSource;

    private PICOLeftJoystickMovement movementController; // 引用 PICOLeftJoystickMovement 脚本
    private float nextPlayTime;

    private void Start()
    {
        // 获取 PICOLeftJoystickMovement 脚本
        movementController = GetComponent<PICOLeftJoystickMovement>();
        if (movementController == null)
        {
            Debug.LogWarning("未找到 PICOLeftJoystickMovement 组件！");
        }
        else
        {
            Debug.LogWarning("成功找到 PICOLeftJoystickMovement 组件！");
        }
    }

    private void FixedUpdate()
    {
        // 判断玩家是否在移动
        if (movementController != null && movementController.IsMoving())
        {
            Debug.LogWarning("玩家正在移动。");

            nextPlayTime += Time.deltaTime; // 计时

            // 射线检测脚下的地面类型
            bool tmp_IsHit = Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit tmp_HitInfo,
                2f // 射线长度，根据玩家高度调整
            );

            if (tmp_IsHit)
            {
                //Debug.LogWarning($"射线检测到地面：{tmp_HitInfo.collider.name}，标签为：{tmp_HitInfo.collider.tag}");

                // 根据地面 Tag 查找对应的脚步声数据
                foreach (var tmp_AudioElement in FootstepAudioData.FootstepAudio)
                {
                    if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag))
                    {
                        //Debug.LogWarning($"地面类型匹配：{tmp_AudioElement.Tag}");

                        float tmp_Delay = tmp_AudioElement.Delay;

                        // 如果达到延迟时间，播放脚步声
                        if (nextPlayTime >= tmp_Delay)
                        {
                            int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                            int tmp_AudioIndex = Random.Range(0, tmp_AudioCount);
                            AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                            FootstepAudioSource.clip = tmp_FootstepAudioClip;
                            FootstepAudioSource.Play();

                            //Debug.LogWarning($"播放脚步声：{tmp_FootstepAudioClip.name}");

                            nextPlayTime = 0; // 重置延迟时间
                            break;
                        }
                        else
                        {
                            //Debug.LogWarning($"脚步声延迟播放，剩余时间：{tmp_Delay - nextPlayTime}");
                        }
                    }
                    else
                    {
                        //Debug.LogWarning($"地面类型不匹配：期望 {tmp_AudioElement.Tag}，实际 {tmp_HitInfo.collider.tag}");
                    }
                }
            }
            else
            {
                //Debug.LogWarning("射线未检测到地面。");
            }
        }
        else
        {
            //Debug.LogWarning("玩家未移动或移动控制器为空。");
        }
    }
}