using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    [Header("所有编钟")]
    public BeClocked[] allClocks; // 所有编钟的列表

    [Header("正确顺序的编钟")]
    public BeClocked[] correctOrderClocks; // 正确顺序的编钟列表

    [Header("为了我省事儿所以请在这里写上最大数量")]
    public int maxNum;

    [Header("调用式公开,别动")]
    public int nowNum = 0; // 当前敲击的正确编钟数量
    public int nowClockedNum = 0; // 当前敲击的编钟总数

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

        // 确保所有编钟和正确顺序编钟已赋值
        if (allClocks == null || allClocks.Length == 0)
        {
            Debug.LogError("All Clocks list is not assigned or empty!");
        }
        if (correctOrderClocks == null || correctOrderClocks.Length == 0)
        {
            Debug.LogError("Correct Order Clocks list is not assigned or empty!");
        }
    }

    public void PlayOnce(BeClocked clock)
    {
        // 播放对应编钟的音效
        PlayClockSound(clock);

        // 判断是否按正确顺序敲击
        if (nowNum < maxNum && clock == correctOrderClocks[nowNum])
        {
            nowNum++;
            nowClockedNum++;

            // 如果全部正确敲击，触发额外逻辑
            if (nowNum == maxNum)
            {
                shotController.ArrowShot();
            }
        }
        else
        {
            // 如果顺序错误，重置计数
            nowNum = 0;
            nowClockedNum = 0;
            Debug.Log("顺序错误，重置计数");
        }
    }

    // 播放编钟音效
    private void PlayClockSound(BeClocked clock)
    {
        if (clockAudioData != null)
        {
            // 查找编钟在 allClocks 列表中的索引
            int clockIndex = System.Array.IndexOf(allClocks, clock);
            if (clockIndex >= 0 && clockIndex < clockAudioData.FootstepAudio.Count)
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
                Debug.LogError("编钟索引超出范围或未找到！");
            }
        }
        else
        {
            Debug.LogError("音效数据库未设置！");
        }
    }
}