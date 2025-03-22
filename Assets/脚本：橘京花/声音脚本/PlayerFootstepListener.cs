using UnityEngine;

public class PlayerFootstepListener : MonoBehaviour
{
    public FootstepAudioData FootstepAudioData;
    public AudioSource FootstepAudioSource;

    private CharacterController characterController;
    private Transform footstepTransform;
    private PICOLeftJoystickMovement movementController; // 引用 PICOLeftJoystickMovement 脚本

    private float nextPlayTime;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = transform;

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
        if (characterController.isGrounded)
        {
            Debug.LogWarning("玩家在地面上。");

            // 判断摇杆输入是否大于死区（表示玩家在移动）
            if (movementController != null && movementController.IsMoving())
            {
                Debug.LogWarning("玩家正在移动。");

                nextPlayTime += Time.deltaTime; // 计时

                // 射线检测脚下的地面类型
                bool tmp_IsHit = Physics.Linecast(
                    footstepTransform.position,
                    footstepTransform.position + Vector3.down * characterController.height / 2,
                    out RaycastHit tmp_HitInfo
                );

                if (tmp_IsHit)
                {
                    Debug.LogWarning($"射线检测到地面：{tmp_HitInfo.collider.name}，标签为：{tmp_HitInfo.collider.tag}");

                    foreach (var tmp_AudioElement in FootstepAudioData.FootstepAudio)
                    {
                        // 检测地面类型是否匹配
                        if (tmp_HitInfo.collider.tag == tmp_AudioElement.Tag)
                        {
                            Debug.LogWarning($"地面类型匹配：{tmp_AudioElement.Tag}");

                            float tmp_Delay = tmp_AudioElement.Delay;

                            // 如果达到延迟时间，播放脚步声
                            if (nextPlayTime >= tmp_Delay)
                            {
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = Random.Range(0, tmp_AudioCount);
                                AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootstepAudioSource.clip = tmp_FootstepAudioClip;
                                FootstepAudioSource.Play();

                                Debug.LogWarning($"播放脚步声：{tmp_FootstepAudioClip.name}");

                                nextPlayTime = 0; // 重置延迟时间
                                break;
                            }
                            else
                            {
                                Debug.LogWarning($"脚步声延迟播放，剩余时间：{tmp_Delay - nextPlayTime}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"地面类型不匹配：期望 {tmp_AudioElement.Tag}，实际 {tmp_HitInfo.collider.tag}");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("射线未检测到地面。");
                }
            }
            else
            {
                Debug.LogWarning("玩家未移动或移动控制器为空。");
            }
        }
        else
        {
            Debug.LogWarning("玩家未在地面上。");
        }
    }
}