using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionPanel : MonoBehaviour
{
    public Text time;
    public Text pb;
    public Text damage;
    public Text balls;
    public Text chance;
    public Text heals;
    public Text shieldHealth;
    public Text dps;
    public Text nibbler;
    public Text grade;
    public ButtonScript returnButton;
    public InfernoManager manager;

    float points;

    void Start()
    {
        returnButton.buttonClicked += ClosePanel;
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateText()
    {
        time.text = Tools.SecondsToMinutes((float)manager.encounterTicks * TickManager.maxTickTime, true);
        pb.text = Tools.SecondsToMinutes(GameManager.scores.fastestTicks * TickManager.maxTickTime, true);
        damage.text = manager.damageTaken.ToString();
        balls.text = manager.ballsTanked.ToString();
        chance.text = (Mathf.Floor(manager.deathChance * 10000) / 100).ToString() + "%";
        heals.text = ZukHealer.zukHeals.ToString();
        shieldHealth.text = manager.shieldHealthValue + "/" + 600;
        dps.text = (Mathf.Floor(((float)manager.damageDealt * 1000 / ((float)manager.encounterTicks * TickManager.maxTickTime))) / 1000).ToString();
        grade.text = CalculateGrade();
        float nibblerChance = 1 / 100;
        if (WornEquipment.slayerHelm)
        {
            nibblerChance = 1 / 75;
        }
        if (Random.Range(0f, 1f) < nibblerChance)
        {
            nibbler.text = "YES! gzzzz";
        }
        else
        {
            nibbler.text = "No.";
        }
    }

    string CalculateGrade()
    {
        float timePoints = Mathf.Max(1300 - (manager.encounterTicks), 0);
        float damagePoints = Mathf.Max(1000 - manager.damageTaken * 3, 0);
        float ballsPoints = Mathf.Max(1000 - manager.ballsTanked * 500, 0);
        float chancePoints = Mathf.Max((manager.deathChance > 0.001) ? 700 - manager.deathChance * 1000 : 1000, 0);
        float healsPoints = Mathf.Max(500 - ZukHealer.zukHeals);

        float total = Mathf.Max(timePoints + damagePoints + ballsPoints + chancePoints + healsPoints, 1);
        Debug.Log(total);

        float Aplus = 3750;
        float Aminus = 2900;
        float Bminus = 2400;
        float Cminus = 1900;
        float D = 1500;
        float[] thresholds = new float[6];
        thresholds[0] = Aplus;
        thresholds[1] = Aminus;
        thresholds[2] = Bminus;
        thresholds[3] = Cminus;
        thresholds[4] = D;
        thresholds[5] = 0;

        int i = 0;
        while (total < thresholds[i])
        {
            i++;
        }

        if (i == 0)
        {
            return "<color=lime>A+</color>";
        }
        if (i == 5)
        {
            return "<color=red>D-</color>";
        }



        string grade = "A";
        string color = "<color=lime>";
        string endColor = "</color>";
        if (i == 2)
        {
            grade = "B";
            color = "<color=yellow>";
        }
        else if (i == 3)
        {
            grade = "C";
            color = "<color=orange>";
        }
        else if (i == 4)
        {
            grade = "D";
            color = "<color=red>";
        }

        string modifier;
        if (i == 1)
        {
            float difference1 = thresholds[0] - thresholds[1];
            float yourDifference1 = total - thresholds[1];
            if (yourDifference1 > difference1 * 0.5f)
            {
                modifier = "";
            }
            else
            {
                modifier = "-";
            }

            return color + grade + modifier + endColor;
        }

        float difference = thresholds[0] - thresholds[1];
        float yourDifference = total - thresholds[1];
        if (yourDifference > difference * 0.66f)
        {
            modifier = "+";
        }
        else if (yourDifference > difference * 0.33f)
        {
            modifier = "";
        }
        else
        {
            modifier = "-";
        }

        return color + grade + modifier + endColor;
    }
}
