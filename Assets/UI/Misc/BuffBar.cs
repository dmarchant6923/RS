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

    public GameObject[] extras;
    public float[] timers;
    public string[] names;

    public GameObject buffTimer;
    public Text buffTimerText;

    public static BuffBar instance;

    private void Start()
    {
        instance = this;
        timers = new float[extras.Length];
        names = new string[extras.Length];
    }

    private void Update()
    {
        for (int i = 0; i < extras.Length; i++)
        {
            if (timers[i] > 0)
            {
                if (Mathf.Floor(timers[i]) > Mathf.Floor(timers[i] - Time.deltaTime))
                {
                    UpdateExtraTimer(i, timers[i] - Time.deltaTime);
                }
                timers[i] -= Time.deltaTime;
            }
            else
            {
                timers[i] = 0;
                names[i] = "";
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
    }

    public void UpdateBaseStats()
    {
        attack.UpdateStats(PlayerStats.currentAttack, PlayerStats.initialAttack);
        strength.UpdateStats(PlayerStats.currentStrength, PlayerStats.initialStrength);
        defense.UpdateStats(PlayerStats.currentDefence, PlayerStats.initialDefence);
        range.UpdateStats(PlayerStats.currentRanged, PlayerStats.initialRanged);
        mage.UpdateStats(PlayerStats.currentMagic, PlayerStats.initialMagic);
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

    public void CreateExtraTimer(Texture texture, float time, string name)
    {
        for (int i = 0; i < extras.Length; i++)
        {
            if (extras[i].activeSelf && names[i] == name)
            {
                if (timers[i] < time)
                {
                    timers[i] = time;
                    extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(time);
                    return;
                }
            }
        }

        for (int i = 0; i < extras.Length; i++)
        {
            if (extras[i].activeSelf == false)
            {
                extras[i].SetActive(true);
                extras[i].transform.GetChild(0).GetComponent<RawImage>().texture = texture;
                extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(time);
                timers[i] = time;
                names[i] = name;
                return;
            }
        }
    }

    public void UpdateExtraTimer(int i, float time)
    {
        extras[i].GetComponentInChildren<Text>().text = Tools.SecondsToMinutes(time);
    }
}
