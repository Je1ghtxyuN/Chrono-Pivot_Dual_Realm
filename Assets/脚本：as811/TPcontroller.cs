using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPcontroller : MonoBehaviour
{
    public Transform player;
    public Transform position;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.position = position.position;
        }
        
    }

}
