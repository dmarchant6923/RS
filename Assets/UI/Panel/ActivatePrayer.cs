using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivatePrayer : MonoBehaviour
{
    Action prayerAction;
    RawImage image;
    [HideInInspector] public bool effectiveActive = false;
    bool active;

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
    bool QPactive;

    private void Start()
    {
        image = GetComponent<RawImage>();
        prayerAction = GetComponent<Action>();
        prayerAction.menuTexts[0] = "Activate <color=orange>" + gameObject.name + "</color>";
        prayerAction.action0 += ClickPrayer;
        if (image.color.a == 1)
        {
            prayerAction.menuTexts[0] = "Deactivate <color=orange>" + gameObject.name + "</color>";
            effectiveActive = true;
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
            QPactive = true;
        }
        selectQuickPrayer = false;
    }

    void ClickPrayer()
    {
        if (selectQuickPrayer == false)
        {
            active = !active;
            ChangeColors(active);
            StartCoroutine(DelayActivate(false));
        }
        else
        {
            QPactive = !QPactive;
            CheckOnOff(QPactive);
            ChangeColors(false);
            StartCoroutine(DelayActivate(true));
        }
    }

    public void ClickQuickPrayer()
    {
        active = !active;
        StartCoroutine(DelayActivate(false));
    }

    IEnumerator DelayActivate(bool QP)
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        if (QP == false)
        {
            Prayer.activateQueue.Add(this);
        }
        else
        {
            Prayer.QPactivateQueue.Add(this);
        }
    }

    public void Activate()
    {
        effectiveActive = !effectiveActive;
        active = effectiveActive;
        ChangeColors(effectiveActive);
        if (effectiveActive)
        {
            AvoidConflictingPrayers(false);
        }
        if (effectiveActive == false && Prayer.CheckActivePrayers().Count == 0)
        {
            PlayerStats.prayersOnForATick = false;
        }
    }

    public void QPActivate()
    {
        QPeffectiveActive = !QPeffectiveActive;
        QPactive = QPeffectiveActive;
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
            effectiveActive = false;
            active = false;
            ChangeColors(false);
        }
        else
        {
            QPeffectiveActive = false;
            QPactive = false;
            CheckOnOff(QPeffectiveActive);
        }
    }

    public void ChangeColors(bool on)
    {
        Color color = image.color;
        if (on == false || selectQuickPrayer)
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

    void CheckOnOff(bool on)
    {
        if (on)
        {
            newCheck.texture = checkOn;
        }
        else
        {
            newCheck.texture = checkOff;
        }
    }

    void AvoidConflictingPrayers(bool QP)
    {
        foreach (GameObject prayer in Prayer.prayers)
        {
            ActivatePrayer script = prayer.GetComponent<ActivatePrayer>();
            if (script == this || (QP == false && script.effectiveActive == false) || (QP && script.QPeffectiveActive == false))
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
