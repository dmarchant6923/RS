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

    [HideInInspector] public bool enoughSpec = true;
    Color enoughSpecColor;
    public Color notEnoughSpecColor;

    public static SpecBar instance;

    bool lightBearer = false;
    [HideInInspector] public int regenTicks = 50;
    [HideInInspector] public int ticks = 0;

    Equipment currentSpecWep;
    bool disableSpec;

    private void Start()
    {
        instance = this;

        specPercentage = 100;
        maxWidth = greenBar.rectTransform.rect.width;
        enoughSpecColor = greenBar.color;

        specAction = GetComponent<Action>();
        specAction.menuTexts[0] = "Use <color=lime>Special Attack</color>";
        specAction.serverAction0 += UpdateSpecActive;
        specAction.orderLevels[0] = -1;

        TickManager.beforeTick += SpecRegen;
        TickManager.afterTick += DisableSpec;

        active = false;
    }

    public void UpdateSpecActive(bool activate)
    {
        if (activate && enoughSpec == false)
        {
            GameLog.Log("You don't have enough Special Attack left.");
            activate = false;
        }

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

    public void UpdateSpecActive()
    {
        UpdateSpecActive(!active);
    }

    public void ToggleOrbEnabled(bool hasSpec)
    {
        if (WornEquipment.weapon == null || WornEquipment.weapon.gameObject != currentSpecWep)
        {
            disableSpec = true;
        }
        currentSpecWep = WornEquipment.weapon;
        specOrb.ToggleEnabled(hasSpec);
    }

    void DisableSpec()
    {
        if (disableSpec)
        {
            disableSpec = false;
            UpdateSpecActive(false);
        }
    }

    void SpecRegen()
    {
        if (specPercentage == 100)
        {
            ticks = 0;
            return;
        }

        if (lightBearer == false && WornEquipment.lightBearer)
        {
            lightBearer = true;
            regenTicks = 25;
            ticks = 0;
        }
        else if (lightBearer && WornEquipment.lightBearer == false)
        {
            lightBearer = false;
            regenTicks = 50;
            ticks = 0;
        }

        if (ticks >= regenTicks)
        {
            specPercentage = Mathf.Min(100, specPercentage + 10);
            ticks = 0;
            CheckSpec();
        }

        ticks++;
    }

    public void CheckSpec()
    {
        if (WornEquipment.weapon != null && WornEquipment.weapon.spec != null)
        {
            if (enoughSpec && specPercentage < WornEquipment.weapon.spec.specCost)
            {
                enoughSpec = false;
                greenBar.color = notEnoughSpecColor;
                UpdateSpecActive(false);
            }
            else if (enoughSpec == false && specPercentage >= WornEquipment.weapon.spec.specCost)
            {
                enoughSpec = true;
                greenBar.color = enoughSpecColor;
            }
            
            if (active && enoughSpec)
            {
                Player.player.combatScript.useSpec = true;
            }
        }

        greenBar.rectTransform.sizeDelta = new Vector2(maxWidth * specPercentage / 100, greenBar.rectTransform.rect.height);
        specText.text = "Special Attack: " + Mathf.Floor(specPercentage) + "%";
        specOrb.UpdateOrb();
    }

    public void UseSpec()
    {
        UpdateSpecActive(false);
        Player.player.combatScript.useSpec = false;
        specPercentage -= WornEquipment.weapon.spec.specCost;
        CheckSpec();
    }

    public static void ResetSpecBar()
    {
        specPercentage = 100;
        instance.CheckSpec();
    }
}
