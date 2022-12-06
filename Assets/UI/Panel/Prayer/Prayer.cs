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
    public static GameObject[] prayerObjects = new GameObject[29];
    public static ActivatePrayer[] prayers = new ActivatePrayer[29];

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

    public static float attackPrayerBonus; //0
    public static float strengthPrayerBonus; //1
    public static float rangedAttackPrayerBonus; //2
    public static float rangedStrengthPrayerBonus; //3
    public static float magicAttackPrayerBonus; //4
    public static float defensePrayerBonus; //5

    public static bool protectFromMagic;
    public static bool protectFromMissiles;
    public static bool protectFromMelee;
    public static bool retribution;
    public static bool redemption;
    public static bool smite;
    public static string currentOverhead;

    public static bool preserve;
    public static bool rapidHeal;

    public static float[] bonuses = new float[6];


    private void Start()
    {
        drainRate = 0;

        for (int i = 0; i < prayerParent.GetComponentsInChildren<ActivatePrayer>().Length; i++)
        {
            prayerObjects[i] = prayerParent.GetComponentsInChildren<ActivatePrayer>()[i].gameObject;
            prayers[i] = prayerObjects[i].GetComponent<ActivatePrayer>();
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
                prayerObjects[index].GetComponent<RectTransform>().position = position;
            }
        }

        TickManager.beforeTick += Activate;
        TickManager.afterTick += UpdateText;
        PlayerStats.newStats += UpdatePrayerReqs;

        quickPrayerBackground.SetActive(false);
        doneButton.SetActive(false);
        prayerText.transform.parent.gameObject.SetActive(true);
        prayerText.text = PlayerStats.currentPrayer + " / " + PlayerStats.initialPrayer;


        for (int i = 0; i < bonuses.Length; i++)
        {
            bonuses[i] = 1;
        }
        attackPrayerBonus = 1;
        strengthPrayerBonus = 1;
        rangedAttackPrayerBonus = 1;
        rangedStrengthPrayerBonus = 1;
        magicAttackPrayerBonus = 1;
        defensePrayerBonus = 1;
    }
    void Activate()
    {
        if (prayerChanged)
        {
            drainRate = 0;

            for (int i = 0; i < bonuses.Length; i++)
            {
                bonuses[i] = 1;
            }
            protectFromMagic = false;
            protectFromMissiles = false;
            protectFromMelee = false;
            retribution = false;
            redemption = false;
            smite = false;

            bool overhead = false;
            foreach (ActivatePrayer prayer in prayers)
            {
                if (prayer.active)
                {
                    drainRate += prayer.drainRate;

                    if (prayer.overhead)
                    {
                        if (prayer.name == "Protect from Melee")
                        {
                            protectFromMelee = true;
                        }
                        else if (prayer.name == "Protect from Missiles")
                        {
                            protectFromMissiles = true;
                        }
                        else if (prayer.name == "Protect from Magic")
                        {
                            protectFromMagic = true;
                        }
                        else if (prayer.name == "Retribution")
                        {
                            retribution = true;
                        }
                        else if (prayer.name == "Redemption")
                        {
                            redemption = true;
                        }
                        else if (prayer.name == "Smite")
                        {
                            smite = true;
                        }
                        currentOverhead = prayer.name;

                        UpdateOverhead(prayer);
                        overhead = true;
                    }
                }
            }

            if (overhead == false)
            {
                UpdateOverhead(null);
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


        foreach (ActivatePrayer prayer in prayers)
        {
            if (prayer.QPeffectiveActive == false)
            {
                continue;
            }

            if (prayer.active == false)
            {
                prayer.ServerClickPrayer();
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
            foreach (ActivatePrayer prayer in prayers)
            {
                prayer.newCheck.enabled = true;
                prayer.selectQuickPrayer = true;
                prayer.ChangeColors(false);
                prayer.CheckOnOff(prayer.QPeffectiveActive);
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
            foreach (ActivatePrayer prayer in prayers)
            {
                prayer.newCheck.enabled = false;
                prayer.selectQuickPrayer = false;
                prayer.ChangeColors(prayer.active);
            }
            prayerText.transform.parent.gameObject.SetActive(true);
        }
    }

    static public void DeactivatePrayers()
    {
        foreach (ActivatePrayer prayer in prayers)
        {
            prayer.ForceDeactivate(false);
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

    public static List<ActivatePrayer> CheckActivePrayers()
    {
        List<ActivatePrayer> activePrayers = new List<ActivatePrayer>();
        foreach (ActivatePrayer prayer in prayers)
        {
            if (prayer.active)
            {
                activePrayers.Add(prayer);
            }
        }
        return activePrayers;
    }

    public static void UpdatePrayerBonuses()
    {
        for (int i = 0; i < bonuses.Length; i++)
        {
            bonuses[i] = 1;
        }

        foreach (ActivatePrayer prayer in CheckActivePrayers())
        {
            for (int i = 0; i < prayer.bonuses.Length; i++)
            {
                if (prayer.bonuses[i] > 1)
                {
                    bonuses[i] = prayer.bonuses[i];
                }
            }
        }

        attackPrayerBonus = bonuses[0];
        strengthPrayerBonus = bonuses[1];
        rangedAttackPrayerBonus = bonuses[2];
        rangedStrengthPrayerBonus = bonuses[3];
        magicAttackPrayerBonus = bonuses[4];
        defensePrayerBonus = bonuses[5];
    }

    void UpdateOverhead(ActivatePrayer prayer)
    {
        if (prayer != null)
        {
            Overhead script;
            if (UIManager.newOverhead == null)
            {
                UIManager.newOverhead = Instantiate(UIManager.staticOverhead, transform.position, Quaternion.identity);
                script = UIManager.newOverhead.GetComponent<Overhead>();
                script.objectWithOverhead = Player.player.gameObject;
                script.worldSpaceOffset = 0.8f;
                script.initialOverhead = currentOverhead;
            }
            else
            {
                script = UIManager.newOverhead.GetComponent<Overhead>();
                script.SwitchOverhead(currentOverhead);
            }
        }
        else
        {
            if (UIManager.newOverhead != null)
            {
                Destroy(UIManager.newOverhead);
            }
        }
    }

    void UpdatePrayerReqs()
    {
        foreach (ActivatePrayer prayer in prayers)
        {
            prayer.SetPrayerUnlocked();
        }
    }
}
