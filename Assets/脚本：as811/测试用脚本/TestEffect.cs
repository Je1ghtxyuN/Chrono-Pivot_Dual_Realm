using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect : MonoBehaviour
{
    public ParticleController[] particleControllers;
    private void OnTriggerEnter(Collider other)
    {
        foreach (var particleController in particleControllers)
        {
            StartCoroutine(particleController.PlayAndStop());
        }
    }
}
