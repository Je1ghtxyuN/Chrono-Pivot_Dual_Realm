using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShot : MonoBehaviour
{
    public ShotController sc;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sc.ArrowShot();
            Debug.Log("按下了空格键");
        }
    }
}
