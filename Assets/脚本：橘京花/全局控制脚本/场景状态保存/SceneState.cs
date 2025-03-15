using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneState
{
    public string sceneName;
    public Dictionary<string, ObjectState> objectStates = new Dictionary<string, ObjectState>();

    // 保存玩家状态
    public PlayerState playerState;

    // 保存物体状态
    public void SaveObjectState(GameObject obj)
    {
        string key = obj.name + "_" + obj.GetInstanceID(); // 使用名称和实例ID作为键
        objectStates[key] = new ObjectState
        {
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale,
            isActive = obj.activeSelf,

            // 保存动画状态（如果物体有 Animator 组件）
            animatorState = GetAnimatorState(obj),
            animatorTrigger = GetAnimatorTrigger(obj),

            // 保存粒子状态（如果物体有 ParticleSystem 组件）
            isParticlePlaying = GetParticleState(obj)
        };
    }

    // 恢复物体状态
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

            // 恢复动画状态
            RestoreAnimatorState(obj, state.animatorState, state.animatorTrigger);

            // 恢复粒子状态
            RestoreParticleState(obj, state.isParticlePlaying);
        }
    }

    // 保存玩家状态
    public void SavePlayerState(GameObject player)
    {
        playerState = new PlayerState
        {
            position = player.transform.position,
            rotation = player.transform.rotation,
            scale = player.transform.localScale,
            isActive = player.activeSelf
        };
    }

    // 恢复玩家状态
    public void RestorePlayerState(GameObject player)
    {
        if (playerState != null)
        {
            player.transform.position = playerState.position;
            player.transform.rotation = playerState.rotation;
            player.transform.localScale = playerState.scale;
            player.SetActive(playerState.isActive);
        }
    }

    // 获取动画状态
    private int GetAnimatorState(GameObject obj)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            return animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
        return 0;
    }

    // 获取动画触发器状态
    private bool GetAnimatorTrigger(GameObject obj)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            return animator.GetBool("RotateForward"); // 根据实际触发器名称修改
        }
        return false;
    }

    // 获取粒子状态
    private bool GetParticleState(GameObject obj)
    {
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            return particleSystem.isPlaying;
        }
        return false;
    }

    // 恢复动画状态
    private void RestoreAnimatorState(GameObject obj, int animatorState, bool animatorTrigger)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(animatorState);
            animator.SetBool("RotateForward", animatorTrigger); // 根据实际触发器名称修改
        }
    }

    // 恢复粒子状态
    private void RestoreParticleState(GameObject obj, bool isParticlePlaying)
    {
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            if (isParticlePlaying)
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

[System.Serializable]
public class ObjectState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;

    // 动画状态
    public int animatorState; // 当前动画状态
    public bool animatorTrigger; // 动画触发器状态

    // 粒子状态
    public bool isParticlePlaying; // 粒子是否正在播放
}

[System.Serializable]
public class PlayerState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;
}