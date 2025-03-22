using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool pour = false; 
    public static bool _throw = false;
 
    public static void SetThrowState(bool newState)
    {
        _throw = newState;
    }
 
    public void SetPourState(bool newState)
    {
        pour = newState;
    }

    private void Update()
    {
        TimetravelController timeController = FindObjectOfType<TimetravelController>();
        if (_throw == true && timeController.GetTime() == false)
        {
            pour = true; 
        }
    }

}