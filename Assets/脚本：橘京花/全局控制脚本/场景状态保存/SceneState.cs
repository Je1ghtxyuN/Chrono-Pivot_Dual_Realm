using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneState
{
    public string sceneName;
    public Dictionary<string, ObjectState> objectStates = new Dictionary<string, ObjectState>();

    public void SaveObjectState(GameObject obj)
    {
        string key = obj.name + "_" + obj.GetInstanceID(); // 使用名称和实例ID作为键
        objectStates[key] = new ObjectState
        {
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale,
            isActive = obj.activeSelf
        };
    }

    public void RestoreObjectState(GameObject obj)
    {
        string key = obj.name + "_" + obj.GetInstanceID();
        if (objectStates.ContainsKey(key))
        {
            ObjectState state = objectStates[key];
            obj.transform.position = state.position;
            obj.transform.rotation = state.rotation;
            obj.transform.localScale = state.scale;
            obj.SetActive(state.isActive);
        }
    }
}

[System.Serializable]
public class ObjectState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;
}