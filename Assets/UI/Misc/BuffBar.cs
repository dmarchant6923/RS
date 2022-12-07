using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffBar : MonoBehaviour
{
    public BuffIcon attack;
    public BuffIcon strength;
    public BuffIcon defense;
    public BuffIcon range;
    public BuffIcon mage;
    public BuffIcon hitpoints;
    public BuffIcon prayer;

    class ExtraTimer
    {
        public string name;
        public string warningText;
        public string endText;
        public float timer = 0;
        public bool warningGiven;
    }

    ExtraTimer[] extraTimers;
    public GameObject[] extras;

    public GameObject buffTimer;
    public Text buffTimerText;

    public static BuffBar instance;

    private void Start()
    {
        instance = this;

        extraTimers = new ExtraTimer[extras.Length];
        for (int i = 0; i < extraTimers.Length; i++)
        {
            extraTimers[i] = new ExtraTimer();
        }

        PlayerStats.newStats += UpdateBaseStats;
        TickManager.afterTick += UpdatePrayHitpoints;
    }

    private void Update()
    {
        for (int i = 0; i < extras.Length; i++)
        {
            if (extraTimers[i].timer > 0)
            {
                if (Mathf.Floor(extraTimers[i].timer) > Mathf.Floor(extraTimers[i].timer - Time.deltaTime))
                {
                    UpdateExtraTimer(i, extraTimers[i].timer - Time.deltaTime);
                }
                extraTimers[i].timer -= Time.deltaTime;
            }
            else if (extras[i].activeSelf)
            {
                GameLog.Log("<color=red>Your " + extraTimers[i].name + " has run out.</color>");
                extraTimers[i] = new ExtraTimer();
                extras[i].SetActive(false);
            }
        }
    }

    public void UpdateStats()
    {
        attack.UpdateStats(PlayerStats.currentAttack);
        strength.UpdateStats(PlayerStats.currentStrength);
        defense.UpdateStats(PlayerStats.currentDefence);
        range.UpdateStats(PlayerStats.currentRanged);
        mage.UpdateStats(PlayerStats.currentMagic);
        hitpoints.UpdatePrayerHitpoints(PlayerStats.currentHitpoints);
        prayer.UpdatePrayerHitpoints(PlayerStats.currentPrayer);
    }

    public void UpdateBaseStats()
    {
        attack.UpdateStats(PlayerStats.currentAttack, PlayerStats.initialAttack);
        strength.UpdateStats(PlayerStats.currentStrength, PlayerStats.initialStrength);
        defense.UpdateStats(PlayerStats.currentDefence, PlayerStats.initialDefence);
        range.UpdateStats(PlayerStats.currentRanged, PlayerStats.initialRanged);
        mage.UpdateStats(PlayerStats.currentMagic, PlayerStats.initialMagic);
        hitpoints.UpdatePrayerHitpoints(PlayerStats.currentHitpoints, PlayerStats.initialHitpoints);
        prayer.UpdatePrayerHitpoints(PlayerStats.currentPrayer, PlayerStats.initialPrayer);
    }

    void UpdatePrayHitpoints()
    {
        hitpoints.UpdatePrayerHitpoints(PlayerStats.currentHitpoints);
        prayer.UpdatePrayerHitpoints(PlayerStats.currentPrayer);
    }

    public void UpdateTimer(float time)
    {
        if (buffTimer.activeSelf == false)
        {
            buffTimer.SetActive(true);
        }
        buffTimerText.text = Tools.SecondsToMinutes(time);
        if (time < 11)
        {
            buffTimerText.color = Color.red;
        }
        else
        {
            buffTimerText.color = Color.white;
        }
    }

    public void DisableTimer()
    {
        buffTimer.SetActive(false);
    }

    public void CreateExtraTimer(Texture texture, float timeSeconds, string name, string warningText, string endText)
    {
        for (int i = 0; i < extraTimers.Length; i++)
        {
            if (extras[i].activeSelf && extraTimers[i].name == name)
            {
                if (extraTimers[i].timer < timeSeconds)
                {
                    extraTimers[i].timer = timeSeconds;
                    extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(timeSeconds);
                    return;
                }
            }
        }

        for (int i = 0; i < extraTimers.Length; i++)
        {
            if (extras[i].activeSelf == false)
            {
                extras[i].SetActive(true);
                extras[i].transform.GetChild(0).GetComponent<RawImage>().texture = texture;
                extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(timeSeconds);
                extraTimers[i].timer = timeSeconds;
                extraTimers[i].name = name;
                return;
            }
        }
    }

    public void CreatePotionTimer(Texture texture, float timeSeconds, string name)
    {
        string warningText = "<color=red>Your " + name + " is about to run out!</color>";
        string endText = "<color=red>Your " + name + " has run out.</color>";
        CreateExtraTimer(texture, timeSeconds, name, warningText, endText);
    }

    public void UpdateExtraTimer(int i, float time)
    {
        extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(time);
        if (extraTimers[i].timer < 11 && extraTimers[i].warningGiven == false)
        {
            extraTimers[i].warningGiven = true;
            GameLog.Log("<color=red>Your " + extraTimers[i].name + " is about to run out!</color>");
        }
    }
}
