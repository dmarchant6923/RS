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
    public Text timeValue;

    public static string folder = "/SaveData/";
    public static string fileName = "Hiscores";
    public static string extension = ".txt";

    public class Hiscores
    {
        public int deaths;
        public int completions;
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
        timeValue.text = "---";
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

    public static void UpdateCompletions()
    {
        scores.completions++;
        SaveScores();
    }

    public static void UpdateFastestTime(int ticks)
    {
        if (ticks < scores.fastestTicks || scores.fastestTicks == 0)
        {
            scores.fastestTicks = ticks;
            SaveScores();
        }
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
