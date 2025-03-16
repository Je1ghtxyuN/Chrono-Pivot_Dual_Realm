using UnityEngine;

public class PlayerFootstepListener : MonoBehaviour
{
    public FootstepAudioData FootstepAudioData;
    public AudioSource FootstepAudioSource;

    private CharacterController characterController;
    private Transform footstepTransform;

    private float nextPlayTime;



    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = transform;
    }

    private void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {
                nextPlayTime += Time.deltaTime;//计时




                bool tmp_IsHit = Physics.Linecast(footstepTransform.position, footstepTransform.position + Vector3.down * characterController.height / 2,
                    out RaycastHit tmp_HitInfo);//前面两个组成一个Ray，第三个是一个碰撞信息类
                                                //#if UNITY_EDITOR
                                                //                Debug.DrawLine(footstepTransform.position, footstepTransform.position + Vector3.down*characterController.height/2,Color.red,0.25f);
                                                //#endif
                if (tmp_IsHit)
                {

                    foreach (var tmp_AudioElement in FootstepAudioData.FootstepAudio)
                    {

                        //if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag))//检测类型
                        if (tmp_HitInfo.collider.tag == tmp_AudioElement.Tag)
                        {
                            float tmp_Delay = 0;

                            tmp_Delay = tmp_AudioElement.Delay;


                            if (nextPlayTime >= tmp_Delay)
                            {
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = Random.Range(0, tmp_AudioCount);
                                AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootstepAudioSource.clip = tmp_FootstepAudioClip;
                                FootstepAudioSource.Play();

                                nextPlayTime = 0;//重置延迟时间
                                break;
                            }

                        }

                    }

                }
            }
        }
    }
}
