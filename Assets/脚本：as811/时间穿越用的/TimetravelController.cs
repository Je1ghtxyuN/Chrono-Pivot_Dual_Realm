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
    [Header("时间插值")]
    public float up=500f;
    //false为现在，true为过去
    private bool time = false;
    

    private void Start()
    {
        past.SetActive(false);
        now.SetActive(true);
        
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
        
        player.transform.Translate(0f, up, 0f);
        RenderSettings.skybox= Instantiate(pastSkyBox);
        past.SetActive(true);
        now.SetActive(false);
        time = true;
       
    }
    private void IntoNow()
    {
      
        player.transform.Translate(0f, -up, 0f);
        RenderSettings.skybox = Instantiate(nowSkyBox);
        now.SetActive(true);
        past.SetActive(false);
        time = false;
       
    }
}
