using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class bianzhongshunxu : MonoBehaviour
{
    public TMP_Text textComponent;
    public string textToDisplay = "(2,1) (1,3) (2,2) (1,1) (2,5) (2,3)";
    public Color textColor = Color.black;
    public float fontSize = 36f;
    // Start is called before the first frame update
    void Start()
    {
        if (textComponent == null)
        {
            // 如果没有指定TMP_Text组件，尝试在当前对象上添加
            textComponent = gameObject.AddComponent<TMP_Text>();

            // 设置TextMeshPro的字体资源
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (fontAsset != null)
            {
                textComponent.font = fontAsset;
            }
        }

        // 设置文字内容
        textComponent.text = textToDisplay;

        // 设置文字颜色
        textComponent.color = textColor;

        // 设置文字大小
        textComponent.fontSize = fontSize;

        // 设置对齐方式为居中
        textComponent.alignment = TextAlignmentOptions.Center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
