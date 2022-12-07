using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLog : MonoBehaviour
{
    public GameObject logText;
    List<RectTransform> logs = new List<RectTransform>();
    public Transform textParent;
    RectTransform rt;
    Vector2 textParentOnPosition;
    float boxOnHeight;
    float boxOffHeight;

    float verticalSpacing;
    float indent;

    public static GameLog instance;

    public OpenCloseButton button;
    public Texture downArrowOff;
    public Texture downArrowOn;
    public Texture upArrowOff;
    public Texture upArrowOn;

    string openText = "Close game log";
    string closedText = "Open game log";

    bool open = true;

    private void Start()
    {
        instance = this;

        verticalSpacing = logText.GetComponent<Text>().preferredHeight * 1.2f;
        indent = 10;
        textParentOnPosition = textParent.transform.position;
        rt = GetComponent<RectTransform>();
        boxOnHeight = rt.rect.height;
        boxOffHeight = button.GetComponent<RectTransform>().rect.height;

        button.buttonClicked += ButtonClicked;
    }

    public static void Log(string text)
    {
        GameObject newLog = Instantiate(instance.logText);
        newLog.transform.SetParent(instance.textParent);
        newLog.GetComponent<Text>().text = text;
        Canvas.ForceUpdateCanvases();
        int lines = newLog.GetComponent<Text>().cachedTextGenerator.lineCount;
        if (lines > 1)
        {
            for (int i = 0; i < lines; i++)
            {
                int start = newLog.GetComponent<Text>().cachedTextGenerator.lines[i].startCharIdx;
                int end = i == lines - 1 ? text.Length : newLog.GetComponent<Text>().cachedTextGenerator.lines[i + 1].startCharIdx;
                string lineText = text.Substring(start, end - start);
                Log(lineText);
            }
            Destroy(newLog);
            return;
        }
        
        for (int i = 0; i < instance.logs.Count; i++)
        {
            if (instance.logs.Count > 7)
            {
                Destroy(instance.logs[0].gameObject);
                instance.logs.RemoveAt(0);
            }
            instance.logs[i].localPosition += Vector3.up * instance.verticalSpacing;
        }

        for (int i = 1; i < lines; i++)
        {

        }

        newLog.GetComponent<RectTransform>().localPosition = new Vector2(instance.indent, 0);
        instance.logs.Add(newLog.GetComponent<RectTransform>());
    }

    void ButtonClicked()
    {
        if (open)
        {
            open = false;
            button.buttonOff = upArrowOff;
            button.buttonOn = upArrowOn;
            button.image.texture = upArrowOn;
            button.buttonAction.menuTexts[0] = closedText;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxOffHeight);
            textParent.position = textParentOnPosition + Vector2.down * 2000;
        }
        else
        {
            open = true;
            button.buttonOff = downArrowOff;
            button.buttonOn = downArrowOn;
            button.image.texture = downArrowOn;
            button.buttonAction.menuTexts[0] = openText;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxOnHeight);
            textParent.position = textParentOnPosition;
        }
    }
}
