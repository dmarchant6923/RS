using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunToggle : MonoBehaviour
{
    Toggle runToggle;
    Player player;
    bool togglePressed = false;
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

        TickManager.onTick += ToggleRun;
        orbManager.onToggle += RunEnergyPressed;
        orbManager.orbAction.menuTexts[0] = "Toggle Run";
    }

    void RunEnergyPressed()
    {
        togglePressed = true;
    }

    void ToggleRun()
    {
        if (togglePressed)
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

        togglePressed = false;
        if (player.runEnergy == 0)
        {
            runToggle.isOn = false;
            orbManager.active = false;
        }

        numberText.text = Mathf.Round(player.runEnergy / 100).ToString();
    }
}
