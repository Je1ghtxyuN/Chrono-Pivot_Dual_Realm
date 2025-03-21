using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public BeClocked[] beClockeds;
    [Header("为了我省事儿所以请在这里写上最大数量")]
    public int maxNum;
    
    [Header("调用式公开,别动")]
    public int nowNum = 0;
    public int nowClockedNum = 0;
    private ShotController shotController;
    private void Start()
    {
        shotController=FindObjectOfType<ShotController>();
    }
    public void PlayOnce()
    {
       if(nowNum< maxNum)
        {
            if (beClockeds[nowNum].isClocked)
            {
                nowNum++;
                nowClockedNum++;
            }
  
            if (nowNum == maxNum)
                shotController.ArrowShot();
        }
       else
        {
            nowNum = 0;
            nowClockedNum = 0;
        }
        
    }
}
