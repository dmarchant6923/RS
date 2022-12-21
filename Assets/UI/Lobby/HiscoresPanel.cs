using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HiscoresPanel : MonoBehaviour
{
    public ButtonScript resetButton;

    public Text deathValue;
    public Text completionValue;
    public Text kdValue;
    public Text lowestLevelValue;
    public Text leastDamageValue;
    public Text timeValue;
    public Text gradeValue;

    public GameObject parent;

    public static HiscoresPanel instance;

    private void Awake()
    {
        instance = this;
        PlayerStats.reinitialize += FindPanel;
    }

    void FindPanel()
    {
        TV tv = FindObjectOfType<TV>();
        if (tv != null)
        {
            tv.statsPanel = parent;
        }
    }
    private void Start()
    {
        resetButton.buttonClicked += ResetScores;

        WriteScores();
    }

    public void WriteScores()
    {
        deathValue.text = GameManager.scores.deaths.ToString();
        completionValue.text = GameManager.scores.completions.ToString();
        kdValue.text = "-";
        if (GameManager.scores.completions > 0)
        {
            kdValue.text = (GameManager.scores.deaths > 0) ? ((float)GameManager.scores.completions / (float)GameManager.scores.deaths).ToString("F3") : GameManager.scores.completions.ToString();
        }
        lowestLevelValue.text = "-";
        if (GameManager.scores.lowestLevel > 0)
        {
            lowestLevelValue.text = GameManager.scores.lowestLevel.ToString();
        }
        leastDamageValue.text = "-";
        if (GameManager.scores.leastDamage > -1)
        {
            leastDamageValue.text = GameManager.scores.leastDamage.ToString();
        }
        timeValue.text = "-";
        if (GameManager.scores.fastestTicks > 0)
        {
            float seconds = (float)GameManager.scores.fastestTicks * TickManager.maxTickTime;
            string text = Tools.SecondsToMinutes(seconds, true, true);
            timeValue.text = text;
        }
        gradeValue.text = "-";
        if (GameManager.scores.highestScore > 0)
        {
            gradeValue.text = CompletionPanel.CalculateGrade(GameManager.scores.highestScore);
        }
    }

    void ResetScores()
    {
        GameManager.instance.ResetScores();
        WriteScores();
    }
}
