using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HiscoresPanel : MonoBehaviour
{
    public ButtonScript resetButton;
    public OpenCloseButton closeButton;

    public Text deathValue;
    public Text completionValue;
    public Text kdValue;
    public Text lowestLevelValue;
    public Text leastDamageValue;
    public Text timeValue;

    public static string folder = "/SaveData/";
    public static string fileName = "Hiscores";
    public static string extension = ".txt";

    public class Hiscores
    {
        public int deaths;
        public int completions;
        public int lowestLevel;
        public int leastDamage = -1;
        public int fastestTicks;
    }
    public static Hiscores scores = new Hiscores();

    private void Awake()
    {
        resetButton.buttonClicked += ResetScores;
        closeButton.buttonClicked += ClosePanel;
        Action.cancel1 += ClosePanel;

        WriteScores();
    }

    void WriteScores()
    {
        string dir = Application.dataPath + folder + fileName + extension;
        if (File.Exists(dir))
        {
            string jsonString = File.ReadAllText(dir);
            scores = JsonUtility.FromJson<Hiscores>(jsonString);
        }
        else
        {
            scores = new Hiscores();
            string jsonString = JsonUtility.ToJson(scores);
            File.WriteAllText(dir, jsonString);
        }

        deathValue.text = scores.deaths.ToString();
        completionValue.text = scores.completions.ToString();
        kdValue.text = "-";
        if (scores.completions > 0)
        {
            kdValue.text = (scores.deaths > 0) ? ((float)scores.completions / (float)scores.deaths).ToString("F3") : scores.completions.ToString();
        }
        lowestLevelValue.text = "-";
        if (scores.lowestLevel > 0)
        {
            lowestLevelValue.text = scores.lowestLevel.ToString();
        }
        leastDamageValue.text = "-";
        if (scores.leastDamage > -1)
        {
            leastDamageValue.text = scores.leastDamage.ToString();
        }
        timeValue.text = "-";
        if (scores.fastestTicks > 0)
        {
            float seconds = (float)scores.fastestTicks * TickManager.maxTickTime;
            string text = Tools.SecondsToMinutes(seconds, true);
            timeValue.text = text;
        }
    }

    void ResetScores()
    {
        scores = new Hiscores();
        SaveScores();
        WriteScores();
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public static void UpdateDeaths()
    {
        scores.deaths++;
        SaveScores();
    }


    public static void UpdateSuccessStats(int level, int damage, int ticks)
    {
        scores.completions++;
        if (level < scores.lowestLevel || scores.lowestLevel == 0)
        {
            scores.lowestLevel = level;
        }
        if (damage < scores.leastDamage || scores.leastDamage == -1)
        {
            scores.leastDamage = damage;
        }
        if (ticks < scores.fastestTicks || scores.fastestTicks == 0)
        {
            scores.fastestTicks = ticks;
        }

        SaveScores();
    }

    public static void SaveScores()
    {
        string dir = Application.dataPath + folder + fileName + extension;
        if (File.Exists(dir))
        {
            string jsonString = JsonUtility.ToJson(scores);
            File.WriteAllText(dir, jsonString);
        }
        else
        {
            string jsonString = JsonUtility.ToJson(scores);
            File.WriteAllText(dir, jsonString);
        }
    }

    private void OnDisable()
    {
        Action.cancel1 -= ClosePanel;
    }
}
