using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTime : MonoBehaviour
{
    public TimetravelController timetravelController;
    private void Start()
    {
        timetravelController = GameObject.FindObjectOfType<TimetravelController>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Time travel!");
            timetravelController.Timetravel();
        }
    }
}
