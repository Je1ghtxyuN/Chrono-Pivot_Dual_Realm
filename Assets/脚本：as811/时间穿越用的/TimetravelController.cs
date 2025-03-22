using System.Collections;
using UnityEngine;

public class TimetravelController : MonoBehaviour
{
    public GameObject player;
    [Header("过去和现在")]
    public GameObject past; // 过去场景
    public Material pastSkyBox; // 过去的天空盒

    public GameObject now; // 现在场景
    public Material nowSkyBox; // 现在的天空盒

    [Header("传送位置")]
    public Transform pastPosition; // 记录过去的位置
    public Transform nowPosition;  // 记录现在的位置
    public Transform startPosition; // 游戏开始时的出生点位置

    public PICOLeftJoystickMovement movementController;

    [Header("摄像机")]
    public Camera mainCamera; // 引用主摄像机

    [Header("时间插值")]
    public float up = 500f;

    // false为现在，true为过去
    private bool time = false;

    public bool GetTime()
    {
        return time;
    }

    private void Start()
    {
        // 初始化场景状态
        past.SetActive(false);
        now.SetActive(true);

        // 初始化玩家位置为出生点位置
        if (startPosition != null)
        {
            player.transform.position = startPosition.position;
        }
        else if (nowPosition != null)
        {
            // 如果出生点位置未设置，则使用现在的位置作为默认出生点
            player.transform.position = nowPosition.position;
        }

        // 确保摄像机已设置
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned in the inspector!");
        }
    }

    private IEnumerator TimetravelWithDelay()
    {
        Debug.Log("加载中...");
        yield return new WaitForSeconds(0);

        // 执行时间旅行逻辑
        if (time == false)
        {
            IntoPast();
        }
        else
        {
            IntoNow();
        }

        // 切换时间状态
        time = !time;
    }

    public void Timetravel()
    {
        StartCoroutine(TimetravelWithDelay());
    }

    private void IntoPast()
    {
        // 传送到过去的位置
        if (pastPosition != null)
        {
            player.transform.position = pastPosition.position;
        }

        // 设置天空盒和场景状态
        ChangeSkybox(pastSkyBox); // 切换到过去的天空盒
        past.SetActive(true);
        now.SetActive(false);

        if (movementController != null)
        {
            movementController.enabled = false; // 禁用移动逻辑
        }

        if (pastPosition != null)
        {
            player.transform.position = pastPosition.position;
        }

        past.SetActive(true);
        now.SetActive(false);

        if (movementController != null)
        {
            movementController.enabled = true; // 启用移动逻辑
        }
    }

    private void IntoNow()
    {
        // 传送到现在的位置
        if (nowPosition != null)
        {
            player.transform.position = nowPosition.position;
        }

        // 设置天空盒和场景状态
        ChangeSkybox(nowSkyBox); // 切换到现在的天空盒
        now.SetActive(true);
        past.SetActive(false);

        if (movementController != null)
        {
            movementController.enabled = false; // 禁用移动逻辑
        }

        if (nowPosition != null)
        {
            player.transform.position = nowPosition.position;
        }

        now.SetActive(true);
        past.SetActive(false);

        if (movementController != null)
        {
            movementController.enabled = true; // 启用移动逻辑
        }
    }

    // 修改摄像机的天空盒
    private void ChangeSkybox(Material skyboxMaterial)
    {
        if (mainCamera != null && skyboxMaterial != null)
        {
            // 获取摄像机的Skybox组件
            Skybox cameraSkybox = mainCamera.GetComponent<Skybox>();
            if (cameraSkybox == null)
            {
                // 如果摄像机没有Skybox组件，则添加一个
                cameraSkybox = mainCamera.gameObject.AddComponent<Skybox>();
            }
            cameraSkybox.material = skyboxMaterial; // 设置天空盒材质
        }
        else
        {
            Debug.LogError("Main Camera or Skybox Material is not assigned!");
        }
    }
}