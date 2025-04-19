using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisAppearController : MonoBehaviour
{
    public Transform obj;
    private void Start()
    {
        obj=GetComponent<Transform>();
    }
    public void DisAppear()
    {
        Destroy(obj.gameObject);
    }
}
