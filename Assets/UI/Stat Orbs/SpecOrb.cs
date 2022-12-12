using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecOrb : MonoBehaviour
{
    [HideInInspector] public StatOrbManager orbManager;
    Toggle orbToggle;

    public Transform regenIndicator;
    float startPosition;
    float distance;

    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();
        orbManager.orbAction.menuTexts[0] = "Use <color=orange>Special Attack</color>";
        orbToggle = GetComponent<Toggle>();
        orbManager.orbAction.serverAction0 += Toggle;

        startPosition = regenIndicator.localPosition.x;
        distance = Mathf.Abs(regenIndicator.localPosition.x) * 2f;
        regenIndicator.gameObject.SetActive(false);

        TickManager.afterTick += UpdateIndicator;
    }

    void Toggle()
    {
        SpecBar.instance.UpdateSpecActive();
    }

    public void ToggleEnabled(bool enabled)
    {
        if (enabled)
        {
            orbManager.canBeToggled = true;
            orbManager.orbAction.menuTexts[0] = "Use <color=orange>Special Attack</color>";
            orbManager.SwitchSprites(orbManager.active);
        }
        else
        {
            orbManager.active = false;
            orbManager.canBeToggled = false;
            orbManager.orbAction.menuTexts[0] = "";
            orbManager.SwitchSprites(false);
        }
    }

    public void UpdateOrb()
    {
        orbManager.number.text = Mathf.Floor(SpecBar.specPercentage).ToString();
    }

    void UpdateIndicator()
    {
        if (regenIndicator.gameObject.activeSelf && SpecBar.specPercentage == 100)
        {
            regenIndicator.gameObject.SetActive(false);
        }
        else if (regenIndicator.gameObject.activeSelf == false && SpecBar.specPercentage != 100)
        {
            regenIndicator.gameObject.SetActive(true);
        }

        regenIndicator.localPosition = new Vector2(startPosition + distance * ((float)SpecBar.instance.ticks / (float)SpecBar.instance.regenTicks), regenIndicator.localPosition.y);
    }
}
