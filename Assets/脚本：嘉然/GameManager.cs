// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool WaterPoured { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¿ç³¡¾°³Ö¾Ã»¯
        }
        else
        {
            Destroy(gameObject);
        }
    }
}