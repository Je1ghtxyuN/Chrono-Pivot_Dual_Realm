using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndController : MonoBehaviour
{
    [Header("文字内容")]
    [TextArea] public string displayText = "你好，冒险者！";
    [Header("显示设置")]
    public float charDelay = 0.1f; // 字符间隔
    public float displayDuration = 3f; // 显示总时长
    [Header("场景设置")]
    public Transform keyItem;
    public TMP_Text uiText;
    private void Start()
    {
        uiText.gameObject.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.transform==keyItem.transform)
            StartCoroutine(DisplayTextAndEnd());
    }
    IEnumerator DisplayTextAndEnd()
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
        SceneManager.LoadScene("EndScene");
    }
}
