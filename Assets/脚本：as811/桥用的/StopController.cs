using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopController : MonoBehaviour
{
    public Rigidbody arrow;


    public void Stop()
    {
        arrow.velocity=new Vector3(0f, 0f, 0f);
    }
}
