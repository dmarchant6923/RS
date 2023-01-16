using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivatePrayer : MonoBehaviour
{
    Action prayerAction;
    RawImage image;
    RawImage backgroundImage;
    Prayer prayerScript;
    [HideInInspector] public bool active = false;

    [HideInInspector] public bool clicked = false;

    public RawImage check;
    [HideInInspector] public RawImage newCheck;
    public Texture checkOff;
    public Texture checkOn;
    public Transform iconsParent;
    public Transform backgroundsParent;
    public GameObject background;

    public bool offensiveMeleeStr;
    public bool offensiveMeleeAtt;
    public bool offensiveRanged;
    public bool offensiveMage;
    public bool defensive;
    public bool overhead;

    public float drainRate;
    public int levelReq;

    [HideInInspector] public bool selectQuickPrayer;
    [HideInInspector] public int isQuickPrayer;
    [HideInInspector] public bool QPeffectiveActive = false;

    public float[] bonuses = new float[6];

    bool locked;

    private IEnumerator Start()
    {
        image = GetComponent<RawImage>();
        prayerAction = GetComponent<Action>();
        prayerScript = FindObjectOfType<Prayer>();
        prayerAction.menuTexts[0] = "Activate <color=orange>" + gameObject.name + "</color>";

        prayerAction.clientAction0 += ClientClickPrayer;
        prayerAction.serverAction0 += ServerClickPrayer;
        prayerAction.clientAction1 += ClientSelectQuickPrayer;
        prayerAction.serverAction1 += ServerSelectQuickPrayer;
        prayerAction.orderLevels[0] = -1;
        prayerAction.orderLevels[1] = -1;

        newCheck = Instantiate(check, transform);
        newCheck.transform.SetParent(iconsParent);
        newCheck.enabled = false;

        SetPrayerUnlocked();

        isQuickPrayer = PlayerPrefs.GetInt(gameObject.name, 0);
        if (isQuickPrayer == 1)
        {
            newCheck.texture = checkOn;
            QPeffectiveActive = true;
        }
        selectQuickPrayer = false;

        TickManager.afterTick += CorrectBackgroundColor;

        yield return null;

        backgroundImage = Instantiate(background, backgroundsParent).GetComponent<RawImage>();
        backgroundImage.transform.position = transform.position;
        if (backgroundImage.color.a == 1)
        {
            prayerAction.menuTexts[0] = "Deactivate <color=orange>" + gameObject.name + "</color>";
            active = true;
        }
    }

    void CorrectBackgroundColor()
    {
        ChangeColors(active);
    }

    public void SetPrayerUnlocked()
    {
        if (PlayerStats.initialPrayer < levelReq)
        {
            if (active)
            {
                ForceDeactivate(false);
            }
            locked = true;
            isQuickPrayer = 0;
            QPeffectiveActive = false;
            PlayerPrefs.SetInt(gameObject.name, 0);
            Color color = new Color(0.2f, 0.2f, 0.2f, 1);
            image.color = color;
        }
        else
        {
            locked = false;
            image.color = Color.white;
        }
    }

    public void ClientClickPrayer()
    {
        if (locked)
        {
            return;
        }
        ChangeColors();
    }
    public void ServerClickPrayer()
    {
        if (locked)
        {
            GameLog.Log("Your prayer level is too low to use this prayer.");
            return;
        }
        if (PlayerStats.currentPrayer == 0 && active == false)
        {
            GameLog.Log("You are out of prayer points.");
            return;
        }

        active = !active;
        ChangeColors(active);
        if (active)
        {
            AvoidConflictingPrayers(false);
        }
        if (active == false && Prayer.CheckActivePrayers().Count == 0)
        {
            PlayerStats.prayersOnForATick = false;
        }
        prayerScript.prayerChanged = true;

        if (gameObject.name == "Preserve")
        {
            Prayer.preserve = active;
        }
        if (gameObject.name == "Rapid Heal")
        {
            Prayer.rapidHeal = active;
        }
    }


    void ClientSelectQuickPrayer()
    {
        if (locked)
        {
            return;
        }
        CheckOnOff();
    }
    void ServerSelectQuickPrayer()
    {
        if (locked)
        {
            GameLog.Log("Your prayer level is too low to use this prayer.");
            return;
        }
        QPeffectiveActive = !QPeffectiveActive;
        CheckOnOff(QPeffectiveActive);
        if (QPeffectiveActive)
        {
            AvoidConflictingPrayers(true);
        }
    }

    public void ForceDeactivate(bool QP)
    {
        if (QP == false)
        {
            if (active)
            {
                ServerClickPrayer();
            }
        }
        else
        {
            QPeffectiveActive = false;
            CheckOnOff(QPeffectiveActive);
        }
    }

    public void QPActivate()
    {
        if (locked)
        {
            return;
        }

        QPeffectiveActive = !QPeffectiveActive;
        CheckOnOff(QPeffectiveActive);
        if (QPeffectiveActive)
        {
            AvoidConflictingPrayers(true);
        }
    }

    public void ChangeColors(bool on)
    {
        if (locked)
        {
            return;
        }

        Color color = backgroundImage.color;
        if (selectQuickPrayer)
        {
            prayerAction.menuTexts[1] = "Toggle <color=orange>" + gameObject.name + "</color>";
            prayerAction.menuTexts[0] = "";
            color.a = 0;
            backgroundImage.color = color;
            return;
        }

        if (on == false)
        {
            prayerAction.menuTexts[0] = "Activate <color=orange>" + gameObject.name + "</color>";
            color.a = 0;
        }
        else
        {
            prayerAction.menuTexts[0] = "Deactivate <color=orange>" + gameObject.name + "</color>";
            color.a = 1;
        }
        backgroundImage.color = color;
    }

    void ChangeColors()
    {
        if (locked)
        {
            return;
        }

        Color color = backgroundImage.color;
        if (color.a == 1)
        {
            prayerAction.menuTexts[0] = "Activate <color=orange>" + gameObject.name + "</color>";
            color.a = 0;
        }
        else
        {
            prayerAction.menuTexts[0] = "Deactivate <color=orange>" + gameObject.name + "</color>";
            color.a = 1;
        }
        backgroundImage.color = color;
    }

    public void CheckOnOff(bool on)
    {
        if (on && locked == false)
        {
            newCheck.texture = checkOn;
        }
        else
        {
            newCheck.texture = checkOff;
        }
        prayerAction.menuTexts[1] = "Toggle <color=orange>" + gameObject.name + "</color>";
        prayerAction.menuTexts[0] = "";
    }

    void CheckOnOff()
    {
        if (newCheck.texture == checkOff)
        {
            CheckOnOff(true);
        }
        else
        {
            CheckOnOff(false);
        }
    }

    void AvoidConflictingPrayers(bool QP)
    {
        foreach (ActivatePrayer script in Prayer.prayers)
        {
            if (script == this || (QP == false && script.active == false) || (QP && script.QPeffectiveActive == false))
            {
                continue;
            }

            if (offensiveMeleeStr)
            {
                if (script.offensiveMeleeStr || script.offensiveMage || script.offensiveRanged)
                {
                    script.ForceDeactivate(QP);                    
                }
            }
            if (offensiveMeleeAtt)
            {
                if (script.offensiveMeleeAtt || script.offensiveMage || script.offensiveRanged)
                {
                    script.ForceDeactivate(QP);
                }
            }
            if (offensiveRanged)
            {
                if (script.offensiveMeleeStr || script.offensiveMeleeAtt || script.offensiveMage || script.offensiveRanged)
                {
                    script.ForceDeactivate(QP);
                }
            }
            if (offensiveMage)
            {
                if (script.offensiveMeleeStr || script.offensiveMeleeAtt || script.offensiveMage || script.offensiveRanged)
                {
                    script.ForceDeactivate(QP);
                }
            }
            if (defensive)
            {
                if (script.defensive)
                {
                    script.ForceDeactivate(QP);
                }
            }
            if (overhead)
            {
                if (script.overhead)
                {
                    script.ForceDeactivate(QP);
                }
            }
        }
    }
}
