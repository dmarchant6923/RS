using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public Text currentText;
    public Text baseText;
    int currentLvl;
    int baseLvl;

    public void UpdateStats(int current)
    {
        currentLvl = current;
        currentText.text = currentLvl.ToString();
        if (currentLvl > baseLvl)
        {
            currentText.color = Color.green;
        }
        else if (currentLvl == baseLvl)
        {
            currentText.color = Color.yellow;
        }
        else if (currentLvl < baseLvl)
        {
            currentText.color = Color.red;
        }
    }

    public void UpdateStats(int current, int baseLevel)
    {
        baseLvl = baseLevel;
        baseText.text = baseLvl.ToString();
        UpdateStats(current);
    }

    public void UpdatePrayerHitpoints(int current)
    {
        currentLvl = current;
        currentText.text = currentLvl.ToString();
        if (currentLvl > baseLvl)
        {
            currentText.color = Color.green;
        }
        else
        {
            float g = (float)current / (float)baseLvl;
            Color color = new Color(1, g, 0);
            currentText.color = color;
        }
    }

    public void UpdatePrayerHitpoints(int current, int baseLevel)
    {
        baseLvl = baseLevel;
        baseText.text = baseLvl.ToString();
        UpdatePrayerHitpoints(current);
    }
}
