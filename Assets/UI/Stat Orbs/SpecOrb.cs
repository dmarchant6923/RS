using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecOrb : MonoBehaviour
{
    [HideInInspector] public StatOrbManager orbManager;
    Toggle orbToggle;
    public SpecBar specBar;
    
    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();
        orbManager.orbAction.menuTexts[0] = "Use <color=orange>Special Attack</color>";
        orbToggle = GetComponent<Toggle>();
        orbManager.orbAction.serverAction0 += Toggle;
    }

    void Toggle()
    {
        specBar.UpdateSpec();
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
}
