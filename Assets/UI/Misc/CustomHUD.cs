using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomHUD : MonoBehaviour
{
    public Text top;
    public Text bottom;
    Vector2 onPosition;
    RectTransform rt;

    public static CustomHUD instance;
    void Start()
    {
        instance = this;
        rt = GetComponent<RectTransform>();
        onPosition = rt.anchoredPosition;
        Deactivate();
    }

    public void Activate(string topText, string bottomText)
    {
        rt.anchoredPosition = onPosition;
        UpdateText(topText, bottomText);
    }

    public void Deactivate()
    {
        rt.anchoredPosition = onPosition + Vector2.up * 500;
    }

    public void UpdateText(string topText, string bottomText)
    {
        top.text = topText;
        bottom.text = bottomText;
    }

    public void UpdateText(string bottomText)
    {
        UpdateText(top.text, bottomText);
    }
}
