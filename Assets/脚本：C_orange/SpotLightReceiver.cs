using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightReceiver : MonoBehaviour
{
    public Light spotLight;
    private bool isOn = false;

    // Start is called before the first frame update
    void Start()
    {
        if(spotLight == null)
        {
            spotLight = GetComponent<Light>();
            if (spotLight == null)
            {
                Debug.LogError("未找到Spot Light组件，请检查！");
            }
        }
        spotLight.enabled = isOn;
    }

    public void ReceiveLaser()
    {
        isOn = !isOn;
        spotLight.enabled = isOn;
    }
    // Update is called once per frame

    void Update()
    {
        
    }
}
