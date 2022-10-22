using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoRetaliate : MonoBehaviour
{
    bool active = false;
    public static bool effectiveActive = false;
    bool clickedButton = false;

    Action retaliateAction;

    public Texture retaliateOff;
    public Texture retaliateOn;
    RawImage image;

    public Text retaliateText;

    private void Start()
    {
        retaliateAction = GetComponent<Action>();
        retaliateAction.menuTexts[0] = "Auto Retaliate";
        retaliateAction.action0 += ClickButton;

        image = GetComponent<RawImage>();

        TickManager.beforeTick += BeforeTick;

        if (PlayerPrefs.GetInt("Auto Retaliate", 0) == 1)
        {
            clickedButton = true;
            active = true;
            BeforeTick();
        }
    }

    void ClickButton()
    {
        Invoke("DelayClickButton", TickManager.simLatency);
    }
    void DelayClickButton()
    {
        active = !active;
        clickedButton = true;
    }
    void BeforeTick()
    {
        if (clickedButton == false)
        {
            return;
        }
        clickedButton = false;

        effectiveActive = active;
        if (effectiveActive)
        {
            image.texture = retaliateOn;
            retaliateText.text = "Auto Retaliate\n(On)";
        }
        else
        {
            image.texture = retaliateOff;
            retaliateText.text = "Auto Retaliate\n(Off)";
        }

        PlayerPrefs.SetInt("Auto Retaliate", (effectiveActive) ? 1 : 0);
    }
}
