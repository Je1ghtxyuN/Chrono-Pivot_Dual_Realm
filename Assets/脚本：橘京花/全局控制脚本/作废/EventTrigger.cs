using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public string eventId; // 每个机关的唯一ID
    public bool isTriggered = false; // 机关的当前状态

    private void Start()
    {
        // 加载机关状态
        isTriggered = EventManager.Instance.GetEventState(eventId);
        UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            EventManager.Instance.SetEventState(eventId, isTriggered);
            UpdateState();
        }
    }

    // 更新机关的状态（例如改变颜色、播放动画等）
    private void UpdateState()
    {
        if (isTriggered)
        {
            // 机关被触发后的逻辑
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            // 机关未被触发时的逻辑
            GetComponent<Renderer>().material.color = Color.red;
        }
    }
}