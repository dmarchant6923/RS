using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecBar : MonoBehaviour
{
    public RawImage greenBar;
    public Text specText;
    public StatOrbManager specOrb;

    float maxWidth;

    public static float specPercentage;

    Action specAction;
    bool active;
    public static bool effectiveActive;
    bool clicked = false;

    private void Start()
    {
        specPercentage = 100;
        maxWidth = greenBar.rectTransform.rect.width;

        specAction = GetComponent<Action>();
        specAction.menuTexts[0] = "Use <color=green>Special Attack</color>";
        specAction.action0 += ClickedSpec;

        TickManager.beforeTick += UpdateClickedSpec;
        active = false;
        effectiveActive = false;
    }

    void ClickedSpec()
    {
        Invoke("DelayClickSpec", TickManager.simLatency);
    }

    void DelayClickSpec()
    {
        active = !active;
        clicked = true;
    }

    void UpdateClickedSpec()
    {
        if (clicked == false)
        {
            return;
        }
        clicked = false;

        effectiveActive = active;
        UpdateSpec();
    }

    public void UpdateSpec()
    {
        if (effectiveActive)
        {
            specText.color = Color.yellow;
            specOrb.active = true;
        }
        else
        {
            specText.color = Color.black;
            specOrb.active = false;
        }
        active = effectiveActive;
    }
}
