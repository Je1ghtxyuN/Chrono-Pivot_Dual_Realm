using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopController : MonoBehaviour
{
    public Rigidbody arrow;
    public float speed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("¼ý"))
        {
            arrow.velocity = new Vector3(0, 0, 0);
        }
    }

}
