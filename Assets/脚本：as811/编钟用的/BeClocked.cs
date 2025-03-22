using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class BeClocked : MonoBehaviour
{
    public bool isClocked=false;
    private ClockController clockController;
    public BeClocked beClocked;
    private void Start()
    {
        clockController=FindObjectOfType<ClockController>();
        beClocked = GetComponent<BeClocked>();
    }
    public void Clock()
    {
        
            Debug.Log("碰到了");
        if (clockController.nowClockedNum < clockController.maxNum)
        {

            if (beClocked == clockController.beClockeds[clockController.nowClockedNum])
            {
                isClocked = true;
                FindObjectOfType<ClockController>().PlayOnce();
                Debug.Log("顺序正确");
            }
            //顺序不对，重置
            else
            {
                isClocked = false;
                clockController.nowClockedNum = 0;
                clockController.nowNum = 0;
                Debug.Log("重置");
            }
        }
        else
        {
            Debug.Log("已完成，请勿重复碰钟表");
        }
        
        
        
    }
}
