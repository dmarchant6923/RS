using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SelectSkillsPanel : MonoBehaviour
{
    public RawImage[] skillImages = new RawImage[7];
    Text[] skillTexts = new Text[7];

    public ButtonScript resetButton;
    public ButtonScript applyButton;
    public GameObject parent;

    public Text totalLevelText;

    int totalLevel;
    Treadmill treadmill;

    BasePanelScript panelScript;


    public class SavedSkills
    {
        public int[] levels = new int[7];
    }
    public SavedSkills savedSkills = new SavedSkills();
    string folder = "/SaveData/";
    string extension = ".txt";
    string fileName = "Skills";
    string dir;


    private void Awake()
    {
        dir = Application.persistentDataPath + folder;
        PlayerStats.reinitialize += FindPanel;
    }

    void FindPanel()
    {
        treadmill = FindObjectOfType<Treadmill>();
        if (treadmill != null)
        {
            treadmill.skillsPanel = parent;
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        InitializeStats();

        applyButton.buttonClicked += ApplySkills;
        resetButton.buttonClicked += SetSkillsTo99;
        
        panelScript = GetComponent<BasePanelScript>();
        panelScript.panelClosed += ResetNumbers;
    }

    public void IncrementSkill(RawImage skillImage, bool increase)
    {
        for (int i = 0; i < 7; i++)
        {
            if (skillImage == skillImages[i])
            {
                savedSkills.levels[i] = increase ? savedSkills.levels[i] + 1 : savedSkills.levels[i] - 1;
                savedSkills.levels[i] = Mathf.Clamp(savedSkills.levels[i], 1, 99);
                skillTexts[i].text = savedSkills.levels[i].ToString();
                skillTexts[i].color = Color.green;
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
                savedSkills.levels[i] = number;
                savedSkills.levels[i] = Mathf.Clamp(savedSkills.levels[i], 1, 99);
                skillTexts[i].text = savedSkills.levels[i].ToString();
                skillTexts[i].color = Color.green;
            }
        }

        SetTotalLevel();
    }

    public void SetSkillsTo99()
    {
        for (int i = 0; i < 7; i++)
        {
            savedSkills.levels[i] = 99;
            skillTexts[i].text = savedSkills.levels[i].ToString();
            skillTexts[i].color = Color.yellow;
        }

        SetTotalLevel();
    }

    void SetTotalLevel()
    {
        totalLevel = 0;
        for (int i = 0; i < 7; i++)
        {
            totalLevel += savedSkills.levels[i];
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
        PlayerStats.initialAttack = savedSkills.levels[0];
        PlayerStats.initialStrength = savedSkills.levels[1];
        PlayerStats.initialDefence = savedSkills.levels[2];
        PlayerStats.initialRanged = savedSkills.levels[3];
        PlayerStats.initialMagic = savedSkills.levels[4];
        PlayerStats.initialHitpoints = savedSkills.levels[5];
        PlayerStats.initialPrayer = savedSkills.levels[6];
        PlayerStats.setInitialStats = savedSkills.levels;

        PlayerStats.ReinitializeStats();


        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(savedSkills);
        File.WriteAllText(fullPath, jsonString);

        GetComponent<BasePanelScript>().ClosePanel();
    }

    void InitializeStats()
    {
        ResetNumbers();
        ApplySkills();
        SetRealTotalLevel();
    }

    void ResetNumbers()
    {
        string fullPath = dir + fileName + extension;
        if (File.Exists(fullPath))
        {
            string jsonString = File.ReadAllText(fullPath);
            savedSkills = JsonUtility.FromJson<SavedSkills>(jsonString);
        }
        else
        {
            savedSkills = new SavedSkills();
            for (int i = 0; i < 7; i++)
            {
                savedSkills.levels[i] = 99;
            }

            string jsonString = JsonUtility.ToJson(savedSkills);
            File.WriteAllText(fullPath, jsonString);
        }

        for (int i = 0; i < 7; i++)
        {
            skillTexts[i] = skillImages[i].GetComponentInChildren<Text>();
            skillTexts[i].text = savedSkills.levels[i].ToString();
            skillTexts[i].color = Color.yellow;
        }

        SetTotalLevel();
    }
}
