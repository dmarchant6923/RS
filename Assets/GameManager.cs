using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
        public float highestScore;
    }
    public static Hiscores scores = new Hiscores();

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        WriteScores();
        DontDestroyOnLoad(gameObject);
    }

    void WriteScores()
    {
        scores = new Hiscores();
        string dir = Application.dataPath + folder + fileName + extension;
        if (File.Exists(dir))
        {
            string jsonString = File.ReadAllText(dir);
            scores = JsonUtility.FromJson<Hiscores>(jsonString);
        }
        else
        {
            string jsonString = JsonUtility.ToJson(scores);
            File.WriteAllText(dir, jsonString);
        }
    }

    public void ResetScores()
    {
        scores = new Hiscores();
        SaveScores();
        WriteScores();
    }

    public static void UpdateDeaths()
    {
        scores.deaths++;
        SaveScores();
    }


    public static void UpdateSuccessStats(int level, int damage, int ticks, float score)
    {
        scores.completions++;
        if (level < scores.lowestLevel || scores.lowestLevel == 0)
        {
            scores.lowestLevel = level;
        }
        if (damage < scores.leastDamage || scores.leastDamage < 0)
        {
            scores.leastDamage = damage;
        }
        if (ticks < scores.fastestTicks || scores.fastestTicks == 0)
        {
            scores.fastestTicks = ticks;
        }
        if (score > scores.highestScore)
        {
            scores.highestScore = score;
        }

        SaveScores();
    }

    public static void SaveScores()
    {
        string dir = Application.dataPath + folder + fileName + extension;
        string jsonString = JsonUtility.ToJson(scores);
        File.WriteAllText(dir, jsonString);
    }
}
