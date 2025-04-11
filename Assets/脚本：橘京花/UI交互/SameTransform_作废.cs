using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameTransform_作废 : MonoBehaviour
{
    public string playerObjectName = "Player"; // 玩家物体的名称
    public string uiControllerObjectName = "UIController"; // UI控制器物体的名称

    private Transform playerTransform; // 玩家的Transform组件
    private Transform uiControllerTransform; // UI控制器的Transform组件

    // Start is called before the first frame update
    void Start()
    {
        // 通过名称获取物体（包括未激活的）
        GameObject playerObject = FindObjectByName(playerObjectName);
        GameObject uiControllerObject = FindObjectByName(uiControllerObjectName);

        // 检查是否成功找到物体
        if (playerObject != null)
        {
            // 获取玩家的Transform组件
            playerTransform = playerObject.GetComponent<Transform>();
            if (playerTransform == null)
            {
                Debug.LogError($"物体 {playerObjectName} 上未找到 Transform 组件。");
            }
            else
            {
                Debug.Log($"成功找到并获取 {playerObjectName} 的 Transform 组件。");
            }
        }
        else
        {
            Debug.LogError($"未找到名称为 {playerObjectName} 的物体。");
        }

        if (uiControllerObject != null)
        {
            // 获取UI控制器的Transform组件
            uiControllerTransform = uiControllerObject.GetComponent<Transform>();
            if (uiControllerTransform == null)
            {
                Debug.LogError($"物体 {uiControllerObjectName} 上未找到 Transform 组件。");
            }
            else
            {
                Debug.Log($"成功找到并获取 {uiControllerObjectName} 的 Transform 组件。");
            }
        }
        else
        {
            Debug.LogError($"未找到名称为 {uiControllerObjectName} 的物体。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 确保Transform组件不为空
        if (playerTransform != null && uiControllerTransform != null)
        {
            // 将UI控制器的位置和旋转设置为与玩家相同
            uiControllerTransform.position = playerTransform.position;
            uiControllerTransform.rotation = playerTransform.rotation;
        }
    }

    // 通过名称查找物体（包括未激活的）
    GameObject FindObjectByName(string name)
    {
        // 获取场景中所有物体（包括未激活的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }

        return null;
    }
}