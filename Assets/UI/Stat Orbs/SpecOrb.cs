using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecOrb : MonoBehaviour
{
    StatOrbManager orbManager;
    Toggle orbToggle;
    public SpecBar specBar;
    bool clicked = false;
    
    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();
        orbManager.orbAction.menuTexts[0] = "Use Special Attack";
        orbToggle = GetComponent<Toggle>();
        orbManager.onToggle += Toggle;
        TickManager.beforeTick += BeforeTick;
    }

    void Toggle()
    {
        clicked = true;
    }
    void BeforeTick()
    {
        if (clicked == false)
        {
            return;
        }
        clicked = false;

        SpecBar.effectiveActive = orbToggle.isOn;
        specBar.UpdateSpec();
    }
}
