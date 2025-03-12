using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VRDebugDisplay : MonoBehaviour
{
    public static VRDebugDisplay Instance;

    [Header("UI Settings")]
    public Canvas debugCanvas;
    public Text debugText;
    public int maxLines = 10;
    public float logLifetime = 2f;

    [Header("Log Filters")]
    [Tooltip("是否显示普通日志")]
    public bool showLogs = true;
    [Tooltip("是否显示警告日志")]
    public bool showWarnings = true;
    [Tooltip("是否显示错误日志")]
    public bool showErrors = true;

    private string logContent = "";
    private Queue<string> logQueue = new Queue<string>();
    private bool isUpdatingLog = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (debugCanvas == null || debugText == null)
        {
            Debug.LogError("Debug Canvas or Debug Text is not assigned!");
            enabled = false;
            return;
        }

        debugText.text = logContent;
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedLog = "";
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                if (!showErrors) return;
                formattedLog = $"<color=red>{logString}</color>";
                break;
            case LogType.Warning:
                if (!showWarnings) return;
                formattedLog = $"<color=yellow>{logString}</color>";
                break;
            default:
                if (!showLogs) return;
                formattedLog = logString;
                break;
        }

        AddLog(formattedLog);
        StartCoroutine(RemoveLogAfterDelay(formattedLog, logLifetime));
    }

    public void AddLog(string message)
    {
        if (logQueue.Count > 0 && logQueue.Last() == message)
        {
            return;
        }

        logQueue.Enqueue(message);

        if (logQueue.Count > maxLines)
        {
            logQueue.Dequeue();
        }

        UpdateLogDisplay();
    }

    private void UpdateLogDisplay()
    {
        if (isUpdatingLog) return;

        isUpdatingLog = true;
        logContent = "";

        foreach (string log in logQueue)
        {
            logContent = log + "\n" + logContent;
        }

        debugText.text = logContent;
        isUpdatingLog = false;
    }

    private IEnumerator RemoveLogAfterDelay(string log, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (logQueue.Contains(log))
        {
            logQueue = new Queue<string>(logQueue.Where(item => item != log).ToArray());
            UpdateLogDisplay();
        }
    }

    private void OnValidate()
    {
        // 当过滤设置变化时清空当前日志
        if (logQueue != null)
        {
            logQueue.Clear();
            UpdateLogDisplay();
        }
    }
}