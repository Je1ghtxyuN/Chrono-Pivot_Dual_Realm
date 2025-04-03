using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShot : MonoBehaviour
{
    public ShotController sc;
    public ArrowCarController ac;
    private void Update()
    {
        if(ac.isIn==true)
        {
            sc.ArrowShot();
            Debug.Log("按下了空格键");
        }
    }
}
