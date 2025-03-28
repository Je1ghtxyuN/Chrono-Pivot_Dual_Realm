using UnityEngine;

public class DestroyOnSpecialCollision : MonoBehaviour
{
    [SerializeField] private string targetTag = "Well";
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            gameManager.SetThrowState(true); 
            Destroy(gameObject);
        }
    }
}