using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunToggle : MonoBehaviour
{
    Toggle runToggle;
    Player player;
    Text numberText;

    StatOrbManager orbManager;

    void Start()
    {
        runToggle = GetComponent<Toggle>();
        player = FindObjectOfType<Player>();
        orbManager = GetComponentInChildren<StatOrbManager>();
        numberText = GetComponentInChildren<Text>();
        numberText.text = Mathf.Round(player.runEnergy / 100).ToString();
        runToggle.isOn = false;

        TickManager.afterTick += AfterTick;
        orbManager.orbAction.menuTexts[0] = "Toggle Run";
        orbManager.orbAction.serverAction0 += ToggleRun;
        orbManager.orbAction.orderLevels[0] = -1;
    }

    void ToggleRun()
    {
        if (player.runEnergy > 100)
        {
            player.runEnabled = orbManager.active;
        }
        else
        {
            runToggle.isOn = false;
            orbManager.active = false;
        }
    }

    void AfterTick()
    {
        if (player.runEnergy == 0)
        {
            runToggle.isOn = false;
            orbManager.active = false;
        }

        numberText.text = Mathf.Round(player.runEnergy / 100).ToString();
    }
}
