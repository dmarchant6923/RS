using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsPanel : MonoBehaviour
{
    public OpenCloseButton closeButton;
    public SettingsCheckmark[] checkmarks = new SettingsCheckmark[8];
    public SettingsNumber latency;
    public GameObject warningText;
    public GameObject parent;

    string folder = "/SaveData/";
    string extension = ".txt";
    string dir;

    public class GameSettings
    {
        public bool[] bools = new bool[8];

        public float latency = 200;
    }

    public GameSettings settings = new GameSettings();

    //final build should be using persistentDataPath, not dataPath
    private void Awake()
    {
        gameObject.SetActive(true);
        parent.SetActive(false);
    }

    IEnumerator Start()
    {
        closeButton.buttonClicked += ClosePanel;
        Action.cancel1 += ClosePanel;
        dir = Application.dataPath + folder;

        InitializeSettings();

        yield return null;

        ApplySettings();
    }

    public void InitializeSettings()
    {
        InitializeSettings(false);
    }

    public void InitializeSettings(bool setToDefault)
    {
        string fileName = "GameSettings";
        string fullPath = dir + fileName + extension;
        if (File.Exists(fullPath) && setToDefault == false)
        {
            string jsonString = File.ReadAllText(fullPath);
            settings = JsonUtility.FromJson<GameSettings>(jsonString);
        }
        else
        {
            settings = new GameSettings();
            settings.bools[0] = true;
            settings.bools[1] = true;
            settings.bools[2] = true;
            settings.bools[3] = true;
            settings.bools[4] = true;
            settings.bools[5] = true;
            settings.bools[6] = false;
            settings.bools[7] = false;
            settings.latency = 200;
            string jsonString = JsonUtility.ToJson(settings);
            File.WriteAllText(fullPath, jsonString);
        }

        for (int i = 0; i < settings.bools.Length; i++)
        {
            checkmarks[i].Check(settings.bools[i]);
        }
        latency.SetValue(settings.latency);
    }

    void SaveSettings()
    {
        for (int i = 0; i < settings.bools.Length; i++)
        {
            settings.bools[i] = checkmarks[i].check;
        }
        settings.latency = latency.value;

        string fileName = "GameSettings";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(settings);
        File.WriteAllText(fullPath, jsonString);
    }

    public void ApplySettings()
    {
        SaveSettings();
        OptionManager.UpdateGameSettings(settings.bools, settings.latency);
        ClosePanel();
    }

    void ClosePanel()
    {
        InitializeSettings();
        parent.SetActive(false);
    }

    public void OpenPanel()
    {
        parent.SetActive(true);
    }

    public void CheckWarning()
    {
        for (int i = 0; i < checkmarks.Length; i++)
        {
            if (checkmarks[i].warning != null && checkmarks[i].check)
            {
                if (warningText.activeSelf == false)
                {
                    warningText.SetActive(true);
                }
                return;
            }
        }

        if (warningText.activeSelf)
        {
            warningText.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Action.cancel1 -= ClosePanel;
    }
}
