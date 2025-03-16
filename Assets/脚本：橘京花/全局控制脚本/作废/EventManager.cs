using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // 用于保存机关状态的字典
    private Dictionary<string, bool> _eventStates = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保证在场景切换时不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            GameObject eventManagerObject = new GameObject("EventManager");//自动创建物体并挂载该脚本
            eventManagerObject.AddComponent<EventManager>();
        }
    }

    // 保存机关状态
    public void SetEventState(string eventId, bool state)
    {
        _eventStates[eventId] = state;
    }

    // 获取机关状态
    public bool GetEventState(string eventId)
    {
        if (_eventStates.ContainsKey(eventId))  
        {
            return _eventStates[eventId];
        }
        return false; // 默认返回false，表示机关未被触发
    }
}