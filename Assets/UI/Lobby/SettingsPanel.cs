using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsPanel : MonoBehaviour
{
    public SettingsCheckmark[] checkmarks = new SettingsCheckmark[9];
    public SettingsNumber latency;
    public SettingsNumber uiScale;
    public SettingsHotkeyButton[] hotkeyButtons = new SettingsHotkeyButton[5];
    public GameObject warningText;
    public GameObject parent;

    string folder = "/SaveData/";
    string extension = ".txt";
    string dir;

    public ButtonScript applyButton;
    public ButtonScript resetButton;

    Canvas canvas;
    public static SettingsPanel instance;

    public Text ping;

    public class GameSettings
    {
        public bool[] bools = new bool[9];

        public float latency = 200;
        public float uiScale;

        public int[] hotkeys = new int[5];
    }

    public GameSettings settings = new GameSettings();

    public static bool panelOpen = false;

    //final build should be using persistentDataPath, not dataPath
    private void Awake()
    {
        instance = this;
        canvas = FindObjectOfType<Canvas>();
        for (int i = 0; i < checkmarks.Length; i++)
        {
            checkmarks[i].checkNumber = i;
        }
        GetComponent<BasePanelScript>().panelClosed += SetPanelOpenBoolToFalse;

        PlayerStats.reinitialize += FindPanel;
    }

    void FindPanel()
    {
        SettingsTable table = FindObjectOfType<SettingsTable>();
        if (table != null)
        {
            table.settingsPanel = this;
        }
    }

    IEnumerator Start()
    {
        dir = Application.persistentDataPath + folder;
        applyButton.buttonClicked += ApplySettings;
        resetButton.buttonClicked += ResetToDefault;

        yield return null;

        InitializeSettings(false);

        ApplySettings();

        yield return null;
        parent.SetActive(true);
    }

    public void ResetToDefault()
    {
        InitializeSettings(true);
        for (int i = 0; i < hotkeyButtons.Length; i++)
        {
            hotkeyButtons[i].SetHotkey(settings.hotkeys[i]);
        }
    }

    public void InitializeSettings(bool setToDefault)
    {
        string fileName = "GameSettings";
        string fullPath = dir + fileName + extension;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            settings = new GameSettings();
            for (int i = 0; i < settings.bools.Length; i++)
            {
                int num = 1;
                if (i > 5) { num = 0; }
                settings.bools[i] = PlayerPrefs.GetInt("settings bools " + i, num) == 1;
            }
            settings.latency = PlayerPrefs.GetFloat("settings latency", 100);
            settings.uiScale = PlayerPrefs.GetFloat("settings uiScale", settings.uiScale = Mathf.Clamp(100 * canvas.pixelRect.width / 1920, 25, 100));

            settings.hotkeys[0] = PlayerPrefs.GetInt("settings hotkeys 0", 1);
            settings.hotkeys[1] = PlayerPrefs.GetInt("settings hotkeys 1", 2);
            settings.hotkeys[2] = PlayerPrefs.GetInt("settings hotkeys 2", -1);
            settings.hotkeys[3] = PlayerPrefs.GetInt("settings hotkeys 3", 3);
            settings.hotkeys[4] = PlayerPrefs.GetInt("settings hotkeys 4", 4);
        }
        else if (File.Exists(fullPath) && setToDefault == false)
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
            settings.bools[8] = false;
            settings.latency = 100;
            settings.uiScale = Mathf.Clamp(100 * canvas.pixelRect.width / 1920, 25, 100);

            settings.hotkeys[0] = 1;
            settings.hotkeys[1] = 2;
            settings.hotkeys[2] = -1;
            settings.hotkeys[3] = 3;
            settings.hotkeys[4] = 4;

            string jsonString = JsonUtility.ToJson(settings);
            File.WriteAllText(fullPath, jsonString);
        }

        for (int i = 0; i < settings.bools.Length; i++)
        {
            checkmarks[i].Check(settings.bools[i]);
        }
        for (int i = 0; i < settings.hotkeys.Length; i++)
        {
            hotkeyButtons[i].currentHotkey = settings.hotkeys[i];
        }
        latency.SetValue(settings.latency);
        uiScale.SetValue(settings.uiScale);

        OptionManager.UpdateGameSettings(settings.bools, settings.latency, settings.uiScale, settings.hotkeys);
    }

    void SaveSettings()
    {
        for (int i = 0; i < settings.bools.Length; i++)
        {
            settings.bools[i] = checkmarks[i].check;
        }
        for (int i = 0; i < settings.hotkeys.Length; i++)
        {
            settings.hotkeys[i] = hotkeyButtons[i].currentHotkey;
        }
        settings.latency = latency.value;
        settings.uiScale = uiScale.value;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            for (int i = 0; i < settings.bools.Length; i++)
            {
                PlayerPrefs.SetInt("settings bools " + i, settings.bools[i] ? 1 : 0);
            }
            PlayerPrefs.SetFloat("settings latency", settings.latency);
            PlayerPrefs.SetFloat("settings uiScale", settings.uiScale);

            PlayerPrefs.SetInt("settings hotkeys 0", settings.hotkeys[0]);
            PlayerPrefs.SetInt("settings hotkeys 1", settings.hotkeys[1]);
            PlayerPrefs.SetInt("settings hotkeys 2", settings.hotkeys[2]);
            PlayerPrefs.SetInt("settings hotkeys 3", settings.hotkeys[3]);
            PlayerPrefs.SetInt("settings hotkeys 4", settings.hotkeys[4]);

            return;
        }

        string fileName = "GameSettings";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(settings);
        File.WriteAllText(fullPath, jsonString);

    }

    public void ApplySettings()
    {
        SaveSettings();
        OptionManager.UpdateGameSettings(settings.bools, settings.latency, settings.uiScale, settings.hotkeys);
        GetComponent<BasePanelScript>().ClosePanel();
        Canvas.ForceUpdateCanvases();
        ping.text = settings.latency + " ms";
    }

    public void OpenPanel()
    {
        parent.SetActive(true);
        InitializeSettings(false);
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

    void SetPanelOpenBoolToFalse()
    {
        panelOpen = false;
    }
}
