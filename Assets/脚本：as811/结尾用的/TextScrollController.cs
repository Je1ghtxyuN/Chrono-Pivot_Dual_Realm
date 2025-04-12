using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScrollController : MonoBehaviour
{
    public Rigidbody text;
    public Transform textposition;
    public float speed=1f;
    private void Start()
    {
        text.velocity = new Vector3(0f, speed, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == text.transform)
        {
            text.velocity = new Vector3(0f, 0f, 0f);
            text.isKinematic = true;
        }
    }
}
