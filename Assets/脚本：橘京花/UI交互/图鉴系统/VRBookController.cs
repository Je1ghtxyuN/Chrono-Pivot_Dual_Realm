using UnityEngine;
using System.Collections.Generic;

public class VRBookController : MonoBehaviour
{
    [Header("页面设置")]
    public List<GameObject> bookPages;
    public int currentPageIndex = 0;

    [Header("调试设置")]
    public bool showDebugLogs = true;

    public void GoToNextPage()
    {
        if (currentPageIndex < bookPages.Count - 1)
        {
            currentPageIndex++;
            if (showDebugLogs) Debug.Log($"切换到下一页，当前页索引: {currentPageIndex}");
            ShowCurrentPage();
        }
        else
        {
            if (showDebugLogs) Debug.Log("已是最后一页，无法继续翻页");
        }
    }

    public void GoToPrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            if (showDebugLogs) Debug.Log($"切换到上一页，当前页索引: {currentPageIndex}");
            ShowCurrentPage();
        }
        else
        {
            if (showDebugLogs) Debug.Log("已是第一页，无法继续翻页");
        }
    }

    public void ExitBook()
    {
        if (showDebugLogs) Debug.Log("正在退出书本...");
        gameObject.SetActive(false);
    }

    private void ShowCurrentPage()
    {
        for (int i = 0; i < bookPages.Count; i++)
        {
            bool isActive = i == currentPageIndex;
            bookPages[i]?.SetActive(isActive);

            if (showDebugLogs && isActive)
            {
                Debug.Log($"显示页面: {bookPages[i].name}");
            }
        }
    }
}