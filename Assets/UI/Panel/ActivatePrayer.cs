using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivatePrayer : MonoBehaviour
{
    Action prayerAction;
    RawImage image;
    Prayer prayerScript;
    [HideInInspector] public bool active = false;

    [HideInInspector] public bool clicked = false;

    public RawImage check;
    [HideInInspector] public RawImage newCheck;
    public Texture checkOff;
    public Texture checkOn;
    public Transform prayerIcons;

    public bool offensiveMeleeStr;
    public bool offensiveMeleeAtt;
    public bool offensiveRanged;
    public bool offensiveMage;
    public bool defensive;
    public bool overhead;

    public float drainRate;

    [HideInInspector] public bool selectQuickPrayer;
    [HideInInspector] public int isQuickPrayer;
    [HideInInspector] public bool QPeffectiveActive = false;

    private void Start()
    {
        image = GetComponent<RawImage>();
        prayerAction = GetComponent<Action>();
        prayerScript = FindObjectOfType<Prayer>();
        prayerAction.menuTexts[0] = "Activate <color=orange>" + gameObject.name + "</color>";

        prayerAction.clientAction0 += ClientClickPrayer;
        prayerAction.serverAction0 += ServerClickPrayer;
        prayerAction.clientAction1 += ClientSelectQuickPrayer;
        prayerAction.serverAction1 += ServerSelectQuickPrayer;
        //order level for both actions is 0


        if (image.color.a == 1)
        {
            prayerAction.menuTexts[0] = "Deactivate <color=orange>" + gameObject.name + "</color>";
            active = true;
        }

        newCheck = Instantiate(check, transform);
        newCheck.transform.SetParent(prayerIcons);
        newCheck.enabled = false;

        isQuickPrayer = PlayerPrefs.GetInt(gameObject.name, 0);
        if (isQuickPrayer == 1)
        {
            newCheck.texture = checkOn;
            QPeffectiveActive = true;
        }
        selectQuickPrayer = false;
    }



    void ClientClickPrayer()
    {
        ChangeColors();
    }
    public void ServerClickPrayer()
    {
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
    }


    void ClientSelectQuickPrayer()
    {
        CheckOnOff();
    }
    void ServerSelectQuickPrayer()
    {
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
        QPeffectiveActive = !QPeffectiveActive;
        CheckOnOff(QPeffectiveActive);
        if (QPeffectiveActive)
        {
            AvoidConflictingPrayers(true);
        }
    }

    public void ChangeColors(bool on)
    {
        Color color = image.color;
        if (selectQuickPrayer)
        {
            prayerAction.menuTexts[1] = "Toggle <color=orange>" + gameObject.name + "</color>";
            prayerAction.menuTexts[0] = "";
            color.a = 0;
            image.color = color;
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
        image.color = color;
    }

    void ChangeColors()
    {
        Color color = image.color;
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
        image.color = color;
    }

    public void CheckOnOff(bool on)
    {
        if (on)
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
            newCheck.texture = checkOn;
        }
        else
        {
            newCheck.texture = checkOff;
        }
        prayerAction.menuTexts[1] = "Toggle <color=orange>" + gameObject.name + "</color>";
        prayerAction.menuTexts[0] = "";
    }

    void AvoidConflictingPrayers(bool QP)
    {
        foreach (GameObject prayer in Prayer.prayers)
        {
            ActivatePrayer script = prayer.GetComponent<ActivatePrayer>();
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
