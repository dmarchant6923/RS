using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSkillsPanel : MonoBehaviour
{
    public RawImage[] skillImages = new RawImage[7];
    public int[] skillLevels = new int[7];
    Text[] skillTexts = new Text[7];

    public ButtonScript resetButton;
    public ButtonScript applyButton;

    public Text totalLevelText;

    int totalLevel;

    private void Start()
    {
        InitializeStats();

        applyButton.buttonClicked += ApplySkills;
        resetButton.buttonClicked += ResetSkills;
    }

    public void IncrementSkill(RawImage skillImage, bool increase)
    {
        for (int i = 0; i < 7; i++)
        {
            if (skillImage == skillImages[i])
            {
                skillLevels[i] = increase ? skillLevels[i] + 1 : skillLevels[i] - 1;
                skillLevels[i] = Mathf.Clamp(skillLevels[i], 1, 99);
                skillTexts[i].text = skillLevels[i].ToString();
            }
        }

        SetTotalLevel();
    }

    public void SetSkill(RawImage skillImage, int number)
    {
        for (int i = 0; i < 7; i++)
        {
            if (skillImage == skillImages[i])
            {
                skillLevels[i] = number;
                skillLevels[i] = Mathf.Clamp(skillLevels[i], 1, 99);
                skillTexts[i].text = skillLevels[i].ToString();
            }
        }

        SetTotalLevel();
    }

    public void ResetSkills()
    {
        for (int i = 0; i < 7; i++)
        {
            skillLevels[i] = 99;
            skillTexts[i].text = skillLevels[i].ToString();
        }

        SetTotalLevel();
    }

    void SetTotalLevel()
    {
        totalLevel = 0;
        for (int i = 0; i < 7; i++)
        {
            totalLevel += skillLevels[i];
        }
        totalLevelText.text = "Total level: " + totalLevel;
    }
    public void SetRealTotalLevel()
    {
        PlayerStats.SetTotalLevel();
        totalLevelText.text = "Total level: " + PlayerStats.totalLevel;
    }


    public void ApplySkills()
    {
        PlayerStats.initialAttack = skillLevels[0];
        PlayerStats.initialStrength = skillLevels[1];
        PlayerStats.initialDefence = skillLevels[2];
        PlayerStats.initialRanged = skillLevels[3];
        PlayerStats.initialMagic = skillLevels[4];
        PlayerStats.initialHitpoints = skillLevels[5];
        PlayerStats.initialPrayer = skillLevels[6];
        PlayerStats.setInitialStats = skillLevels;

        PlayerStats.ReinitializeStats();

        GetComponent<BasePanelScript>().ClosePanel();
    }

    void InitializeStats()
    {
        skillLevels[0] = PlayerStats.initialAttack;
        skillLevels[1] = PlayerStats.initialStrength;
        skillLevels[2] = PlayerStats.initialDefence;
        skillLevels[3] = PlayerStats.initialRanged;
        skillLevels[4] = PlayerStats.initialMagic;
        skillLevels[5] = PlayerStats.initialHitpoints;
        skillLevels[6] = PlayerStats.initialPrayer;

        for (int i = 0; i < 7; i++)
        {
            skillTexts[i] = skillImages[i].GetComponentInChildren<Text>();
            skillTexts[i].text = skillLevels[i].ToString();
        }

        SetRealTotalLevel();
    }

    private void OnEnable()
    {
        SetTotalLevel();
    }

    private void OnDisable()
    {
        InitializeStats();
    }
}
