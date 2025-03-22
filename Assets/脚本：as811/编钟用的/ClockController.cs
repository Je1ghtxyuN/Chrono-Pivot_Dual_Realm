using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public BeClocked[] beClockeds;
    [Header("为了我省事儿所以请在这里写上最大数量")]
    public int maxNum;

    [Header("调用式公开,别动")]
    public int nowNum = 0;
    public int nowClockedNum = 0;

    private ShotController shotController;
    private AudioSource audioSource; // 用于播放音效的 AudioSource

    [Header("声音数据库")]
    public FootstepAudioData clockAudioData; // 音效数据库

    private void Start()
    {
        shotController = FindObjectOfType<ShotController>();

        // 初始化 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component could not be added!");
        }

        // 确保音效数据库已赋值
        if (clockAudioData == null)
        {
            Debug.LogError("Clock Audio Data is not assigned!");
        }
    }

    public void PlayOnce()
    {
        if (nowNum < maxNum)
        {
            if (beClockeds[nowNum].isClocked)
            {
                // 播放对应编钟的音效
                PlayClockSound(nowClockedNum);

                nowNum++;
                nowClockedNum++;
            }

            if (nowNum == maxNum)
            {
                shotController.ArrowShot();
            }
        }
        else
        {
            nowNum = 0;
            nowClockedNum = 0;
        }
    }

    // 播放编钟音效
    private void PlayClockSound(int clockIndex)
    {
        if (clockAudioData != null && clockIndex < clockAudioData.FootstepAudio.Count)
        {
            var footstepAudio = clockAudioData.FootstepAudio[clockIndex];
            if (footstepAudio.AudioClips.Count > 0)
            {
                // 随机选择一个音效播放（如果有多个音效）
                int randomIndex = Random.Range(0, footstepAudio.AudioClips.Count);
                AudioClip clip = footstepAudio.AudioClips[randomIndex];

                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                    Debug.Log("播放音效: " + clip.name);
                }
                else
                {
                    Debug.LogError("音效文件为空！");
                }
            }
            else
            {
                Debug.LogError("编钟音效列表为空！");
            }
        }
        else
        {
            Debug.LogError("音效数据库未设置或编钟索引超出范围！");
        }
    }
}