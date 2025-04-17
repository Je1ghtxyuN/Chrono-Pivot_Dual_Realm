using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    public Rigidbody arrow;
    public Transform arrowsposition;
    public float speed;
    public ArrowCarController ac;
    public AudioSource audioSource;
    public bool isflying;
    public Quaternion targetRot = Quaternion.LookRotation(-Vector3.forward);
    public void ArrowShot()
    {
        ac.isIn = false;
        arrow.useGravity = false;
        arrow.velocity = new Vector3(0, 0, speed);
        audioSource.Play();
        isflying = true;
        Debug.Log("Éä¼ý");
    }
    private void Update()
    {
        if (isflying)
        {
            arrow.velocity = new Vector3(0, 0, speed);
            arrowsposition.rotation = targetRot;
        }
    }

}
