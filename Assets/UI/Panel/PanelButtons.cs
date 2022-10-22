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
    Transform currentButton;
    Transform[] buttons = new Transform[5];

    public GameObject panel;
    public GameObject attackStylesPanel;
    public GameObject inventoryPanel;
    public GameObject wornEquipmentPanel;
    public GameObject prayerPanel;
    public GameObject spellbookPanel;
    GameObject[] panelObjects = new GameObject[5];

    Vector3 onPosition;
    Vector3 panelOnPosition;

    Action buttonAction;

    private IEnumerator Start()
    {
        buttons[0] = attackStyles;
        buttons[1] = inventory;
        buttons[2] = wornEquipment;
        buttons[3] = prayer;
        buttons[4] = spellbook;

        panelObjects[0] = attackStylesPanel;
        panelObjects[1] = inventoryPanel;
        panelObjects[2] = wornEquipmentPanel;
        panelObjects[3] = prayerPanel;
        panelObjects[4] = spellbookPanel;

        onPosition = attackStylesPanel.GetComponent<RectTransform>().localPosition;
        panelOnPosition = panel.GetComponent<RectTransform>().localPosition;

        buttonAction = GetComponent<Action>();

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
    }

    public void ForceOpen(string buttonName)
    {
        foreach (Transform button in buttons)
        {
            if (button.name == buttonName)
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
                //panelobject.SetActive(false);
                panelobject.GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
            }
        }
        panel.GetComponent<RectTransform>().localPosition = panelOnPosition + Vector3.right * 1000;
        //panel.SetActive(false);
    }

    public void OnClick(Transform selectedButton, bool forceOpen)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().sprite = offSprite;
            if (panelObjects[i] != null)
            {
                //panelObjects[i].SetActive(false);
                panelObjects[i].GetComponent<RectTransform>().localPosition = onPosition + Vector3.right * 1000;
            }
        }
        if (selectedButton == currentButton && forceOpen == false)
        {
            selectedButton.GetComponent<Image>().sprite = offSprite;
            currentButton = null;
            panel.GetComponent<RectTransform>().localPosition = panelOnPosition + Vector3.right * 1000;
            //panel.SetActive(false);
        }
        else
        {
            selectedButton.GetComponent<Image>().sprite = onSprite;
            currentButton = selectedButton;
            //panel.SetActive(true);
            panel.GetComponent<RectTransform>().localPosition = panelOnPosition;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (currentButton == buttons[i])
            {
                if (panelObjects[i] != null)
                {
                    panelObjects[i].GetComponent<RectTransform>().localPosition = onPosition;
                    //panelObjects[i].SetActive(true);
                }
                break;
            }
        }
    }
}
