using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelButtons : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;

    public Transform attackStyles;
    public Transform inventory;
    public Transform wornEquipment;
    public Transform prayer;
    public Transform spellbook;
    public Transform logout;
    Transform currentButton;
    Transform[] buttons = new Transform[6];
    float buttonHeight;

    public GameObject panel;
    public GameObject attackStylesPanel;
    public GameObject inventoryPanel;
    public GameObject wornEquipmentPanel;
    public GameObject prayerPanel;
    public GameObject spellbookPanel;
    public GameObject logoutPanel;
    GameObject[] panelObjects = new GameObject[6];
    bool panelOpen = false;

    Vector3 onPosition;
    Vector3 panelOnPosition;

    public static bool enablePanelHotkeys;
    public static KeyCode attackStylesHotKey;
    public static KeyCode inventoryHotKey;
    public static KeyCode wornEquipmentHotKey;
    public static KeyCode prayerHotKey;
    public static KeyCode spellbookHotKey;
    static KeyCode[] hotkeys = new KeyCode[5];

    public static PanelButtons instance;

    public RectTransform statsPanel;
    Vector2 panelOpenStatsPosition;

    private IEnumerator Start()
    {
        instance = this;

        buttons[0] = attackStyles;
        buttons[1] = inventory;
        buttons[2] = wornEquipment;
        buttons[3] = prayer;
        buttons[4] = spellbook;
        buttons[5] = logout;

        buttonHeight = attackStyles.GetComponent<RectTransform>().rect.height;

        panelObjects[0] = attackStylesPanel;
        panelObjects[1] = inventoryPanel;
        panelObjects[2] = wornEquipmentPanel;
        panelObjects[3] = prayerPanel;
        panelObjects[4] = spellbookPanel;
        panelObjects[5] = logoutPanel;

        onPosition = attackStylesPanel.GetComponent<RectTransform>().localPosition;
        panelOnPosition = panel.GetComponent<RectTransform>().position;
        panelOpenStatsPosition = statsPanel.anchoredPosition;

        foreach (GameObject panelobject in panelObjects)
        {
            if (panelobject != null)
            {
                panelobject.SetActive(true);
                //panelobject.GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
            }
        }

        yield return null;

        ForceClose();

        hotkeys[0] = attackStylesHotKey;
        hotkeys[1] = inventoryHotKey;
        hotkeys[2] = wornEquipmentHotKey;
        hotkeys[3] = prayerHotKey;
        hotkeys[4] = spellbookHotKey;
    }

    void Update()
    {
        if (enablePanelHotkeys && Input.anyKeyDown && SettingsPanel.panelOpen == false)
        {
            for (int i = 0; i < hotkeys.Length; i++)
            {
                if (Input.GetKeyDown(hotkeys[i]))
                {
                    OnClick(buttons[i], true);
                }
            }
        }
    }

    public void ForceOpen(string buttonName)
    {
        foreach (Transform button in buttons)
        {
            if (button.name.ToLower() == buttonName.ToLower())
            {
                OnClick(button, true);
                return;
            }
        }
    }
    public void ForceClose()
    {
        currentButton = null;
        foreach (GameObject panelobject in panelObjects)
        {
            if (panelobject != null)
            {
                panelobject.GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
            }
        }
        panel.GetComponent<RectTransform>().position = panelOnPosition + Vector3.right * 1000;
    }

    public void OnClick(Transform selectedButton, bool forceOpen)
    {
        if (selectedButton == buttons[5])
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                panelObjects[i].GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
            }
            panelObjects[5].GetComponent<RectTransform>().localPosition = onPosition;
            currentButton = selectedButton;
            panel.GetComponent<RectTransform>().position = panelOnPosition;
            panelOpen = true;
            return;
        }

        panelObjects[5].GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
        for (int i = 0; i < buttons.Length - 1; i++)
        {
            buttons[i].GetComponent<Image>().sprite = offSprite;
            panelObjects[i].GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
        }
        if (selectedButton == currentButton && forceOpen == false)
        {
            selectedButton.GetComponent<Image>().sprite = offSprite;
            currentButton = null;
            panel.GetComponent<RectTransform>().position = panelOnPosition + Vector3.right * 1000;
            panelOpen = false;
        }
        else
        {
            selectedButton.GetComponent<Image>().sprite = onSprite;
            currentButton = selectedButton;
            panel.GetComponent<RectTransform>().position = panelOnPosition;
            panelOpen = true;
        }

        for (int i = 0; i < buttons.Length - 1; i++)
        {
            if (currentButton == buttons[i])
            {
                panelObjects[i].GetComponent<RectTransform>().localPosition = onPosition;
                break;
            }
        }

        if (panelOpen)
        {
            statsPanel.anchoredPosition = panelOpenStatsPosition;
        }
        else
        {
            statsPanel.anchoredPosition = Vector2.up * panelOpenStatsPosition.y;
        }
    }

    public void ResetPanelPosition()
    {
        panelOnPosition = new Vector2(panelOnPosition.x, buttonHeight * transform.localScale.y * FindObjectOfType<Canvas>().scaleFactor);
    }

    public static void SetHotkeys(int[] keys)
    {
        for (int i = 0; i < hotkeys.Length; i++)
        {
            if (keys[i] != -1)
            {
                KeyCode newKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + keys[i]);
                hotkeys[i] = newKeyCode;
            }
            else
            {
                hotkeys[i] = KeyCode.None;
            }
        }

        attackStylesHotKey = hotkeys[0];
        inventoryHotKey = hotkeys[1];
        wornEquipmentHotKey = hotkeys[2];
        prayerHotKey = hotkeys[3];
        spellbookHotKey = hotkeys[4];
    }
}
