using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimetravelController : MonoBehaviour
{
    public GameObject player;
    [Header("过去和现在")]
    public GameObject past;
    public Material pastSkyBox;

    public GameObject now;
    public Material nowSkyBox;

    [Header("传送位置")]
    public Transform pastPosition; // 记录过去的位置
    public Transform nowPosition;  // 记录现在的位置

    [Header("时间插值")]
    public float up = 500f;

    // false为现在，true为过去
    private bool time = false;

    private void Start()
    {
        past.SetActive(false);
        now.SetActive(true);

        // 初始化玩家位置为现在的位置
        if (nowPosition != null)
        {
            player.transform.position = nowPosition.position;
        }
    }

    public void Timetravel()
    {
        if (time == false)
        {
            IntoPast();
        }
        else
        {
            IntoNow();
        }
    }

    private void IntoPast()
    {
        // 传送到过去的位置
        if (pastPosition != null)
        {
            player.transform.position = pastPosition.position;
        }

        // 设置天空盒和场景状态
        RenderSettings.skybox = Instantiate(pastSkyBox);
        past.SetActive(true);
        now.SetActive(false);
        time = true;
    }

    private void IntoNow()
    {
        // 传送到现在的位置
        if (nowPosition != null)
        {
            player.transform.position = nowPosition.position;
        }

        // 设置天空盒和场景状态
        RenderSettings.skybox = Instantiate(nowSkyBox);
        now.SetActive(true);
        past.SetActive(false);
        time = false;
    }
}