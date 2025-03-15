using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{
    [Header("基础设置")]
    public float duration = 5f; // 持续时间
    public GameObject particleOrigin;
    public ParticleSystem targetParticle;
    
    void Start()
    {
        if (targetParticle != null)
        {
            
        }
        else
        {
            Debug.LogError("哥们你粒子呢？", this);
        }
    }
    //该协程从外部调用，用于播放粒子系统
    public IEnumerator PlayAndStop()
    {
        // 开始播放
        targetParticle.Play();

        // 等待指定时间
        yield return new WaitForSeconds(duration);

        // 停止发射新粒子
        targetParticle.Stop();

        // 等待现有粒子自然消失
        while (targetParticle.particleCount > 0)
        {
            yield return null;
        }


    }
}