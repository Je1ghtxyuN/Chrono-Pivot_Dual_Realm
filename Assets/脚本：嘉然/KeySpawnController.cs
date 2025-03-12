// KeySpawnController.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeySpawnController : MonoBehaviour
{
    public GameObject keyPrefab; // 钥匙预制体
    public Transform spawnPoint; // 第二个水井上的生成点

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.WaterPoured)
        {
            Instantiate(keyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}