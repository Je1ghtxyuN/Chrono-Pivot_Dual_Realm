using System.Diagnostics;
using UnityEngine;

public class SimpleReceiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            // 当检测到激光时输出信息
            UnityEngine.Debug.Log("大吕，姑洗，夹钟，黄钟，仲吕");
        }
    }
}