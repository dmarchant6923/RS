using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prayer : MonoBehaviour
{
    public RectTransform panel;
    public PanelButtons panelButtons;
    float panelScale;

    public GameObject prayerParent;
    public static GameObject[] prayers = new GameObject[29];

    float panelWidthScale = 0.89f;
    float panelHeightScale = 0.92f;
    float brWidthScale = 2f;
    float brHeightScale = 2.91f;

    public GameObject quickPrayerBackground;
    public GameObject doneButton;
    public StatOrbManager prayerOrb;

    public bool prayerChanged = false;
    public static float drainRate;

    public Text prayerText;


    private void Start()
    {
        for (int i = 0; i < prayerParent.GetComponentsInChildren<RawImage>().Length; i++)
        {
            prayers[i] = prayerParent.GetComponentsInChildren<RawImage>()[i].gameObject;
        }

        panelScale = panel.transform.localScale.x;
        Vector2 panelAnchor = panel.position;
        float panelWidth = panel.rect.width * panelScale;
        float panelHeight = panel.rect.height * panelScale;
        Vector2 center = panelAnchor + new Vector2(-panelWidth / 2, panelHeight / 2);
        float widthIncrement = panelWidth * panelWidthScale / 5;
        float heightIncrement = panelHeight * panelHeightScale / 7;
        Vector2 br = center += new Vector2(widthIncrement * brWidthScale, -heightIncrement * brHeightScale);

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                int index = (j * 5) + (i % 5);
                if (index > 28)
                {
                    break;
                }
                float height = br.y + (6 - j) * heightIncrement;
                float width = br.x - (4 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);
                prayers[index].GetComponent<RectTransform>().position = position;
            }
        }

        TickManager.beforeTick += Activate;
        TickManager.afterTick += UpdateText;

        quickPrayerBackground.SetActive(false);
        doneButton.SetActive(false);
        prayerText.transform.parent.gameObject.SetActive(true);
        prayerText.text = PlayerStats.currentPrayer + " / " + PlayerStats.initialPrayer;
    }
    void Activate()
    {
        if (prayerChanged)
        {
            drainRate = 0;
            foreach (GameObject prayer in prayers)
            {
                if (prayer.GetComponent<ActivatePrayer>().active)
                {
                    drainRate += prayer.GetComponent<ActivatePrayer>().drainRate;
                }
            }

            prayerChanged = false;
        }
    }


    public void ActivateQuickPrayers(bool orbActive)
    {
        if (orbActive == false)
        {
            DeactivatePrayers();
            return;
        }


        foreach (GameObject prayer in prayers)
        {
            ActivatePrayer script = prayer.GetComponent<ActivatePrayer>();
            if (script.QPeffectiveActive == false)
            {
                continue;
            }

            if (script.active == false)
            {
                script.ServerClickPrayer();
            }
        }
    }
    public void SelectQuickPrayers()
    {
        if (quickPrayerBackground.activeSelf == false)
        {
            panelButtons.ForceOpen("Prayer");
            quickPrayerBackground.SetActive(true);
            doneButton.SetActive(true);
            foreach (GameObject prayer in prayers)
            {
                prayer.GetComponent<ActivatePrayer>().newCheck.enabled = true;
                prayer.GetComponent<ActivatePrayer>().selectQuickPrayer = true;
                prayer.GetComponent<ActivatePrayer>().ChangeColors(false);
                prayer.GetComponent<ActivatePrayer>().CheckOnOff(prayer.GetComponent<ActivatePrayer>().QPeffectiveActive);
            }
            prayerText.transform.parent.gameObject.SetActive(false);
        }
    }
    public void DeselectQuickPrayers()
    {
        if (quickPrayerBackground.activeSelf)
        {
            quickPrayerBackground.SetActive(false);
            doneButton.SetActive(false);
            foreach (GameObject prayer in prayers)
            {
                prayer.GetComponent<ActivatePrayer>().newCheck.enabled = false;
                prayer.GetComponent<ActivatePrayer>().selectQuickPrayer = false;
                prayer.GetComponent<ActivatePrayer>().ChangeColors(prayer.GetComponent<ActivatePrayer>().active);
            }
            prayerText.transform.parent.gameObject.SetActive(true);
        }
    }

    static public void DeactivatePrayers()
    {
        foreach (GameObject prayer in prayers)
        {
            prayer.GetComponent<ActivatePrayer>().ForceDeactivate(false);
        }
        drainRate = 0;
    }

    void UpdateText()
    {
        prayerText.text = PlayerStats.currentPrayer + " / " + PlayerStats.initialPrayer;
        if (prayerOrb.active && CheckActivePrayers().Count == 0)
        {
            prayerOrb.ClientClickedToggle();
        }
    }

    public static List<GameObject> CheckActivePrayers()
    {
        List<GameObject> activePrayers = new List<GameObject>();
        foreach (GameObject prayer in prayers)
        {
            if (prayer.GetComponent<ActivatePrayer>().active)
            {
                activePrayers.Add(prayer);
            }
        }
        return activePrayers;
    }
}
