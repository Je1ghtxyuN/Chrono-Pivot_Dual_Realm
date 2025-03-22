using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI; // 需要引用UI组件

public class SimpleReceiver : MonoBehaviour
{
    [Header("解密设置")]
    [SerializeField] Text successText;          // 需要拖拽赋值UI文本组件
    [SerializeField] float displayDuration = 2f; // 成功提示显示时间
    [SerializeField] bool showDebugMessages = true; // 调试日志开关

    private Renderer _renderer;
    private Color _originalColor;
    private bool _isActivated;

    void Start()
    {
        // 初始化组件
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;

        // 初始化UI
        if (successText != null)
        {
            successText.gameObject.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogWarning("未分配成功提示文本组件！");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser") && !_isActivated)
        {
            ActivateReceiver();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Laser") && _isActivated)
        {
            DeactivateReceiver();
        }
    }

    private void ActivateReceiver()
    {
        _isActivated = true;

        // 视觉反馈
        _renderer.material.color = Color.green;

        // 显示UI提示
        if (successText != null)
        {
            successText.text = "成功解密！";
            successText.gameObject.SetActive(true);

            // 自动隐藏提示
            Invoke("HideSuccessText", displayDuration);
        }

        // 触发事件（可扩展）
        if (showDebugMessages) UnityEngine.Debug.Log("解密成功触发！");

        // 这里可以添加其他解密成功后的逻辑，例如：
        // - 播放音效
        // - 打开门/机关
        // - 加载新场景
    }

    private void DeactivateReceiver()
    {
        _isActivated = false;
        _renderer.material.color = _originalColor;

        if (showDebugMessages) UnityEngine.Debug.Log("激光离开接收器");
    }

    private void HideSuccessText()
    {
        if (successText != null)
        {
            successText.gameObject.SetActive(false);
        }
    }

    // 编辑器可视化辅助
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}