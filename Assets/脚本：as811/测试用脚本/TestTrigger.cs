using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    public ArrowCarController ac;
    public ShotController sc;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (ac.isIn)
        {
            sc.ArrowShot();
            Debug.Log("¿ªÊ¼ÁË");
        }

    }
}
