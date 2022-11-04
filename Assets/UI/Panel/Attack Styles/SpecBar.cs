using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecBar : MonoBehaviour
{
    public RawImage greenBar;
    public Text specText;
    public SpecOrb specOrb;

    float maxWidth;

    public static float specPercentage;

    Action specAction;
    public static bool active;
    bool clicked = false;

    private void Start()
    {
        specPercentage = 100;
        maxWidth = greenBar.rectTransform.rect.width;

        specAction = GetComponent<Action>();
        specAction.menuTexts[0] = "Use <color=lime>Special Attack</color>";
        specAction.serverAction0 += UpdateSpec;
        specAction.orderLevels[0] = -1;

        active = false;
    }

    public void UpdateSpec(bool activate)
    {
        if (activate)
        {
            active = true;
            specText.color = Color.yellow;
            specOrb.orbManager.active = true;
        }
        else
        {
            active = false;
            specText.color = Color.black;
            specOrb.orbManager.active = false;
        }
    }

    public void UpdateSpec()
    {
        active = !active;
        if (active)
        {
            specText.color = Color.yellow;
            specOrb.orbManager.active = true;
        }
        else
        {
            specText.color = Color.black;
            specOrb.orbManager.active = false;
        }
    }

    public void ToggleOrbEnabled(bool hasSpec)
    {
        UpdateSpec(false);
        specOrb.ToggleEnabled(hasSpec);
    }
}
