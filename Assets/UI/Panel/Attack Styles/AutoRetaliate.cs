using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoRetaliate : MonoBehaviour
{
    public static bool active = false;

    Action retaliateAction;

    public Texture retaliateOff;
    public Texture retaliateOn;
    RawImage image;

    public Text retaliateText;

    private void Start()
    {
        retaliateAction = GetComponent<Action>();
        retaliateAction.menuTexts[0] = "Auto Retaliate";
        //retaliateAction.action0 += ClickButton;
        retaliateAction.serverAction0 += BeforeTickAutoRetailateSwitch;
        retaliateAction.orderLevels[0] = -1;

        image = GetComponent<RawImage>();

        if (PlayerPrefs.GetInt("Auto Retaliate", 0) == 1)
        {
            active = true;
            BeforeTickAutoRetailateSwitch();
        }
    }

    void BeforeTickAutoRetailateSwitch()
    {
        active = !active;
        if (active)
        {
            image.texture = retaliateOn;
            retaliateText.text = "Auto Retaliate\n(On)";
        }
        else
        {
            image.texture = retaliateOff;
            retaliateText.text = "Auto Retaliate\n(Off)";
        }

        PlayerPrefs.SetInt("Auto Retaliate", (active) ? 1 : 0);
    }
}
