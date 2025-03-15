using UnityEngine;
using System.Collections;

public class TimeTravelEffect : MonoBehaviour
{
    [Header("光效设置")]
    public Light targetLight;          // 需要控制的光源组件
    public float peakIntensity = 15f;   // 最大光强
    public float fadeInTime = 1f;       // 变亮时长
    public float holdTime = 0.5f;       // 保持最大亮度时间
    public float fadeOutTime = 2f;      // 变暗时长

    private float originalIntensity;   // 原始光强
    private Material[] originalMaterials; // 原始材质数组

    void Start()
    {
        // 记录初始状态
        originalIntensity = targetLight.intensity;
    }

    // 外部触发接口（可通过事件、碰撞、按键等方式调用）
    public void TriggerEffect()
    {
        StopAllCoroutines();
        StartCoroutine(EffectSequence());
    }

    IEnumerator EffectSequence()
    {
        // 第一阶段：光强渐强
        yield return StartCoroutine(ChangeLightIntensity(
            targetLight.intensity,
            peakIntensity,
            fadeInTime
        ));

       

        // 第四阶段：光强渐弱
        yield return StartCoroutine(ChangeLightIntensity(
            peakIntensity,
            originalIntensity,
            fadeOutTime
        ));
      
    }

    // 渐变光强协程
    IEnumerator ChangeLightIntensity(float from, float to, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            targetLight.intensity = Mathf.Lerp(
                from,
                to,
                Mathf.SmoothStep(0, 1, timer / duration)
            );
            timer += Time.deltaTime;
            yield return null;
        }
        targetLight.intensity = to;
    }

    
}