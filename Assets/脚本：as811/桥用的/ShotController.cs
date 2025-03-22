using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    public Rigidbody arrow;
    public float speed;
   
    public void ArrowShot()
    {
        arrow.velocity = new Vector3(0, 0, -speed);
        Debug.Log("Éä¼ý");
    }
    
}
