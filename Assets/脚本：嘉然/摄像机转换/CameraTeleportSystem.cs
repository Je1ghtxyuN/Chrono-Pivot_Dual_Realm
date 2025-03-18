using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class CameraTeleportSystem : MonoBehaviour
{
    [Header("XR References")]
    public Transform xrOrigin;
    public Transform vrCamera;

    [Header("传送位置配置")]
    public Transform positionWhenPourFalse;  // pour为false时的传送点
    public Transform positionWhenPourTrue;   // pour为true时的传送点

    [Header("过渡效果")]
    public float fadeDuration = 0.5f;

    [Header("输入设置")]
    public KeyCode desktopConfirmKey = KeyCode.Space;

    // 私有变量
    private Image fadeImage;
    private bool inTriggerZone = false;

    void Start()
    {
        InitializeComponents();
        CreateFadeCanvas();
    }

    void InitializeComponents()
    {
        if (xrOrigin == null)
            xrOrigin = GameObject.Find("XR Origin").transform;

        if (vrCamera == null)
            vrCamera = Camera.main.transform;
    }

    void Update()
    {
        if (inTriggerZone && CheckConfirmInput())
        {
            StartCoroutine(TeleportSequence());
        }
    }

    IEnumerator TeleportSequence()
    {
        // 获取目标位置
        Transform targetPosition = GameManager.pour ? positionWhenPourTrue : positionWhenPourFalse;

        // 淡出效果
        yield return StartCoroutine(FadeEffect(Color.black, fadeDuration));

        // 执行传送
        xrOrigin.position = targetPosition.position;
        xrOrigin.rotation = targetPosition.rotation;

        // 强制锁定摄像机
        LockCameraPosition(targetPosition);

        // 淡入效果
        yield return StartCoroutine(FadeEffect(Color.clear, fadeDuration));
    }

    void LockCameraPosition(Transform target)
    {
        // 禁用Character Controller防止碰撞
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            xrOrigin.position = target.position;
            cc.enabled = true;
        }

        // 限制摄像机旋转轴
        Vector3 newRotation = vrCamera.eulerAngles;
        vrCamera.eulerAngles = new Vector3(newRotation.x, 0, 0);
    }

    #region 辅助功能
    void CreateFadeCanvas()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.rectTransform.SetParent(canvas.transform, false);
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        fadeImage.color = Color.clear;
    }

    bool CheckConfirmInput()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return OVRInput.GetDown(OVRInput.Button.One);
#else
        return Input.GetKeyDown(desktopConfirmKey);
#endif
    }

    IEnumerator FadeEffect(Color targetColor, float duration)
    {
        float timer = 0;
        Color startColor = fadeImage.color;

        while (timer < duration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = targetColor;
    }
    #endregion

    #region 触发检测
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            inTriggerZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inTriggerZone = false;
    }
    #endregion
}