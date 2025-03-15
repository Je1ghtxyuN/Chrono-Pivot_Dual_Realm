using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance { get; private set; }

    private Dictionary<string, Dictionary<string, ObjectState>> sceneStates = new Dictionary<string, Dictionary<string, ObjectState>>();
    private Dictionary<string, PlayerState> playerStates = new Dictionary<string, PlayerState>(); // ±£´æÍæ¼Ò×´Ì¬

    private void Awake()
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

    public void SaveCurrentSceneState()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Dictionary<string, ObjectState> objectStates = new Dictionary<string, ObjectState>();

        // ±£´æ³¡¾°ÖÐËùÓÐÎïÌåµÄ×´Ì¬
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.activeInHierarchy)
            {
                ObjectState state = new ObjectState
                {
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    scale = obj.transform.localScale,
                    isActive = obj.activeSelf
                };

                // ±£´æ¶¯»­×´Ì¬
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    state.animatorState = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    state.animatorTrigger = animator.GetBool("RotateForward");
                }

                // ±£´æÁ£×Ó×´Ì¬
                ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    state.isParticlePlaying = particleSystem.isPlaying;
                }

                objectStates[obj.name] = state;
            }
        }

        sceneStates[sceneName] = objectStates;

        // ±£´æÍæ¼Ò×´Ì¬
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerState playerState = new PlayerState
            {
                position = player.transform.position,
                rotation = player.transform.rotation,
                scale = player.transform.localScale,
                isActive = player.activeSelf
            };

            playerStates[sceneName] = playerState;
            Debug.LogWarning("Íæ¼Ò×´Ì¬ÒÑ±£´æ£º" + playerState.position);
        }
    }

    public void RestoreSceneState(string sceneName)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            Dictionary<string, ObjectState> objectStates = sceneStates[sceneName];
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (objectStates.ContainsKey(obj.name))
                {
                    ObjectState state = objectStates[obj.name];
                    obj.transform.position = state.position;
                    obj.transform.rotation = state.rotation;
                    obj.transform.localScale = state.scale;
                    obj.SetActive(state.isActive);

                    // »Ö¸´¶¯»­×´Ì¬
                    Animator animator = obj.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.Play(state.animatorState);
                        animator.SetBool("RotateForward", state.animatorTrigger);
                    }

                    // »Ö¸´Á£×Ó×´Ì¬
                    ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                    if (particleSystem != null)
                    {
                        if (state.isParticlePlaying)
                        {
                            particleSystem.Play();
                        }
                        else
                        {
                            particleSystem.Stop();
                        }
                    }
                }
            }
        }

        // »Ö¸´Íæ¼Ò×´Ì¬
        if (playerStates.ContainsKey(sceneName))
        {
            PlayerState playerState = playerStates[sceneName];
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = playerState.position;
                player.transform.rotation = playerState.rotation;
                player.transform.localScale = playerState.scale;
                player.SetActive(playerState.isActive);
                Debug.LogWarning("Íæ¼Ò×´Ì¬ÒÑ»Ö¸´£º" + playerState.position);
            }
        }
    }

    private class ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool isActive;

        public int animatorState; // ¶¯»­×´Ì¬
        public bool animatorTrigger; // ¶¯»­´¥·¢Æ÷×´Ì¬
        public bool isParticlePlaying; // Á£×Ó²¥·Å×´Ì¬
    }

    private class PlayerState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool isActive;
    }
}