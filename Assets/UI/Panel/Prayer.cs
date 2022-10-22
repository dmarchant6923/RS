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

    [HideInInspector] public bool selectQuickPrayers = false;
    public GameObject quickPrayerBackground;
    public GameObject doneButton;
    public static List<ActivatePrayer> QPactivateQueue = new List<ActivatePrayer>();

    public static List<ActivatePrayer> activateQueue = new List<ActivatePrayer>();
    public static float drainRate;

    public Text prayerText;


    private void Start()
    {
        for (int i = 0; i < prayerParent.GetComponentsInChildren<RawImage>().Length; i++)
        {
            prayers[i] = prayerParent.GetComponentsInChildren<RawImage>()[i].gameObject;
        }
        activateQueue = new List<ActivatePrayer>();
        QPactivateQueue = new List<ActivatePrayer>();

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
                //newPrayer = Instantiate(prayer, prayerParent.transform);
                //newPrayer.GetComponent<RectTransform>().position = position;
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
        if (selectQuickPrayers)
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
                }
                prayerText.transform.parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < QPactivateQueue.Count; i++)
            {
                QPactivateQueue[i].QPActivate();
            }
            QPactivateQueue = new List<ActivatePrayer>();
        }
        else if (quickPrayerBackground.activeSelf)
        {
            quickPrayerBackground.SetActive(false);
            doneButton.SetActive(false);
            foreach (GameObject prayer in prayers)
            {
                prayer.GetComponent<ActivatePrayer>().newCheck.enabled = false;
                prayer.GetComponent<ActivatePrayer>().selectQuickPrayer = false;
                prayer.GetComponent<ActivatePrayer>().ChangeColors(prayer.GetComponent<ActivatePrayer>().effectiveActive);
            }
            prayerText.transform.parent.gameObject.SetActive(true);
        }

        bool change = false;
        for (int i = 0; i < activateQueue.Count; i++)
        {
            change = true;
            activateQueue[i].Activate();
        }
        activateQueue = new List<ActivatePrayer>();
        if (change == false)
        {
            return;
        }

        drainRate = 0;
        foreach (GameObject prayer in prayers)
        {
            if (prayer.GetComponent<ActivatePrayer>().effectiveActive)
            {
                drainRate += prayer.GetComponent<ActivatePrayer>().drainRate;
            }
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

    public void ActivateQuickPrayers()
    {
        bool prayersOn = true;
        foreach (GameObject prayer in prayers)
        {
            ActivatePrayer script = prayer.GetComponent<ActivatePrayer>();
            if (script.effectiveActive == false && script.QPeffectiveActive)
            {
                prayersOn = false;
                break;
            }
        }

        foreach (GameObject prayer in prayers)
        {
            ActivatePrayer script = prayer.GetComponent<ActivatePrayer>();
            if (script.QPeffectiveActive == false)
            {
                continue;
            }

            if (prayersOn && script.effectiveActive)
            {
                script.ClickQuickPrayer();
            }
            else if (prayersOn == false && script.effectiveActive == false)
            {
                script.ClickQuickPrayer();
            }
        }
    }

    void UpdateText()
    {
        prayerText.text = PlayerStats.currentPrayer + " / " + PlayerStats.initialPrayer;
    }

    public static List<GameObject> CheckActivePrayers()
    {
        List<GameObject> activePrayers = new List<GameObject>();
        foreach (GameObject prayer in prayers)
        {
            if (prayer.GetComponent<ActivatePrayer>().effectiveActive)
            {
                activePrayers.Add(prayer);
            }
        }
        return activePrayers;
    }
}
