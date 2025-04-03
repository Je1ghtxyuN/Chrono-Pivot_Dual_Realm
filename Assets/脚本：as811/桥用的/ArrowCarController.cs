using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCarController : MonoBehaviour
{
    public Transform aim;
    public Rigidbody arrow;
    public Transform arrowsposition;
    public Quaternion arrowsrotation = Quaternion.LookRotation(Vector3.up);
   public bool isIn;
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform==arrowsposition.transform)
        {
            arrowsposition.position = aim.position;
            arrowsposition.rotation = arrowsrotation;
            arrow.velocity = new Vector3(0, 0, 0);
            arrow.useGravity = false;
            isIn = true;
            Debug.Log("½øÀ´ÁË");
        }
    }
    private void Update()
    {
        if (isIn)
        {
            arrowsposition.position = aim.position;
            arrowsposition.rotation = arrowsrotation;
        }
    }
}
