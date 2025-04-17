using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BasicSceneSwitch : MonoBehaviour
{
    [Header("rayInteractor")]
    public XRRayInteractor rayInteractor;

    [Header("≥°æ∞…Ë÷√")]
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        //BasicSceneLoading();
    }

    // Update is called once per frame
    void Update()
    {

        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            BasicSceneLoading();
        }
    }

    public void BasicSceneLoading()
    {
        SceneManager.LoadScene("01formal8");
    }
}
