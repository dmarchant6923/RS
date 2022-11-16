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
        currentLvl = current;
        baseLvl = baseLevel;
        currentText.text = currentLvl.ToString();
        baseText.text = baseLvl.ToString();
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
}
