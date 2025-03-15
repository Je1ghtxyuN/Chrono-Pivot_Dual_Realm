using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    private Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保不被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在实例，销毁新的实例
        }
    }

    // 保存当前场景的状态
    public void SaveCurrentSceneState()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Saving state for scene: {sceneName}");
        SceneState sceneState = new SceneState { sceneName = sceneName };

        // 递归保存所有对象
        SaveObjectStateRecursive(sceneState, GameObject.Find("RootObject")); // 从根对象开始
        sceneStates[sceneName] = sceneState;
    }

    private void SaveObjectStateRecursive(SceneState sceneState, GameObject obj)
    {
        if (obj == null) return;

        // 保存当前对象的状态
        sceneState.SaveObjectState(obj);

        // 递归保存所有子对象
        foreach (Transform child in obj.transform)
        {
            SaveObjectStateRecursive(sceneState, child.gameObject);
        }
    }

    // 恢复指定场景的状态
    public void RestoreSceneState(string sceneName)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            Debug.Log($"Restoring state for scene: {sceneName}");
            SceneState sceneState = sceneStates[sceneName];

            // 递归恢复所有对象
            RestoreObjectStateRecursive(sceneState, GameObject.Find("RootObject")); // 从根对象开始
        }
        else
        {
            Debug.LogWarning($"No saved state found for scene: {sceneName}");
        }
    }

    private void RestoreObjectStateRecursive(SceneState sceneState, GameObject obj)
    {
        if (obj == null) return;

        // 恢复当前对象的状态
        sceneState.RestoreObjectState(obj);

        // 递归恢复所有子对象
        foreach (Transform child in obj.transform)
        {
            RestoreObjectStateRecursive(sceneState, child.gameObject);
        }
    }

    // 手动恢复玩家对象的状态
    public void RestorePlayerState(GameObject player)
    {
        string key = player.name + "_" + player.GetInstanceID();
        if (sceneStates.ContainsKey(SceneManager.GetActiveScene().name))
        {
            SceneState sceneState = sceneStates[SceneManager.GetActiveScene().name];
            if (sceneState.objectStates.ContainsKey(key))
            {
                ObjectState state = sceneState.objectStates[key];
                player.transform.position = state.position;
                player.transform.rotation = state.rotation;
                player.transform.localScale = state.scale;
                player.SetActive(state.isActive);
            }
        }
    }
}