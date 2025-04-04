using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    [Header("文字内容")]
    [TextArea] public string displayText = "你好，冒险者！";

    [Header("显示设置")]
    public float charDelay = 0.1f; // 字符间隔
    public float displayDuration = 3f; // 显示总时长

    public TMP_Text uiText;
    public Transform playerTransform; // 玩家的Transform

    private Coroutine displayCoroutine;
    private bool hasBeenTriggered = false; // 新增：标记是否已被触发过

    void Start()
    {
        uiText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 使文本始终面向玩家
        if (uiText.gameObject.activeSelf)
        {
            uiText.transform.LookAt(playerTransform);
            uiText.transform.rotation = Quaternion.LookRotation(uiText.transform.position - playerTransform.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered) // 修改：检查是否已被触发
        {
            StartDisplay();
            Debug.Log("Player entered the trigger");
        }
    }

    public void StartDisplay()
    {
        if (hasBeenTriggered) return; // 新增：如果已经触发过则直接返回

        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(DisplayText());
        hasBeenTriggered = true; // 新增：标记为已触发
        Debug.Log("编钟答案显示");
    }

    IEnumerator DisplayText()
    {
        uiText.gameObject.SetActive(true);
        uiText.text = "";

        // 逐字显示
        foreach (char c in displayText)
        {
            uiText.text += c;
            yield return new WaitForSeconds(charDelay);
            GetComponent<AudioSource>().Play();
        }

        // 保持显示displayDuration秒
        yield return new WaitForSeconds(displayDuration);

        // 淡出
        uiText.gameObject.SetActive(false);
    }

    // 新增：重置触发状态的方法（如果需要）
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}