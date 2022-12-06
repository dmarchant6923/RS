using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackStyles : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject autocastPanel;

    public Text weaponText;
    public Text combatLvlText;
    public Text categoryText;

    public GameObject style1;
    public GameObject style2;
    public GameObject style3;
    public GameObject style4;
    public GameObject mageStyle5;
    public GameObject mageStyle6;
    public GameObject mageStyle7;
    public GameObject mageStyleDefensiveCast8;
    public GameObject mageStyleCast9;

    public RawImage defensiveCastImage;
    public RawImage normalCastImage;
    Texture defaultDefensiveCastTexture;
    Texture defaultNormalCastTexture;

    public GameObject specBar;

    public Texture styleOff;
    public Texture styleOn;

    public Texture bluntPound;
    public Texture bluntPummel;
    public Texture bluntSmash;

    public Texture bowAccurate;
    public Texture bowRapid;
    public Texture bowLongrange;

    public Texture cbowAccurate;
    public Texture cbowRapid;
    public Texture cbowLongrange;

    public Texture whipFlick;
    public Texture whipLash;

    public Texture spearBlock;
    public Texture spearLunge;
    public Texture spearPound;
    public Texture spearSwipe;

    public Texture swordBlock;
    public Texture swordChop;
    public Texture swordLunge;
    public Texture swordSlash;

    public Texture thrownAccurate;
    public Texture thrownLongrange;
    public Texture thrownRapid;

    public Texture unarmedBlock;
    public Texture unarmedKick;
    public Texture unarmedPunch;

    GameObject[] styles = new GameObject[9];

    public static int selectedStyle = 1;
    public bool styleChanged = false;

    public static int attBonus;
    public static int strBonus;
    public static int defBonus;
    public static int rangedBonus;
    public static int distanceBonus;
    public static int rapidBonus;
    public static int magicBonus;

    public static string attackStyle;
    public static string attackType;

    public static string crushStyle = "Crush";
    public static string stabStyle = "Stab";
    public static string slashStyle = "Slash";
    public static string rangedStyle = "Ranged";
    public static string magicStyle = "Magic";

    public static string accurateType = "Accurate";
    public static string aggressiveType = "Aggressive";
    public static string defensiveType = "Defensive";
    public static string controlledType = "Controlled";
    public static string rapidType = "Rapid";
    public static string longrangeType = "Longrange";

    [HideInInspector] public static bool autocastSelectActive = false;
    [HideInInspector] public static bool selectingDefensive = false;
    [HideInInspector] public static Spell currentSpellOnAutocast;

    public static AttackStyles instance;


    private IEnumerator Start()
    {
        instance = this;

        currentSpellOnAutocast = null;
        styles[0] = style1; styles[1] = style2; styles[2] = style3; styles[3] = style4;
        styles[4] = mageStyle5; styles[5] = mageStyle6; styles[6] = mageStyle7; styles[7] = mageStyleDefensiveCast8; styles[8] = mageStyleCast9;

        Inventory.ReadEquippedItems += UpdateWeapon;
        TickManager.beforeTick += UpdateStyleSelected;
        combatLvlText.text = "Combat Lvl: " + PlayerStats.combatLevel.ToString("F2");

        autocastPanel.transform.localPosition = Vector2.right * 1000;

        defaultDefensiveCastTexture = defensiveCastImage.texture;
        defaultNormalCastTexture = normalCastImage.texture;

        yield return null;

        UpdateWeapon();
    }

    public void UpdateWeapon()
    {
        bool hasSpec = false;
        if (WornEquipment.weapon != null)
        {
            weaponText.text = WornEquipment.weapon.name;
            categoryText.text = "Category: " + WornEquipment.weapon.weaponCategory;
            if (WornEquipment.weapon.spec != null)
            {
                hasSpec = true;
            }
        }
        else
        {
            weaponText.text = "Unarmed";
            categoryText.text = "Category: Unarmed";
        }
        specBar.SetActive(hasSpec);
        specBar.GetComponent<SpecBar>().ToggleOrbEnabled(hasSpec);

        UpdateStylesWeaponChange();
    }

    void UpdateStyleSelected()
    {
        ChangeStyleColor(selectedStyle);

        if (styleChanged == false)
        {
            return;
        }
        styleChanged = false;

        if (WornEquipment.weapon != null)
        {
            PlayerPrefs.SetInt(WornEquipment.weapon.weaponCategory, selectedStyle);
        }
        else
        {
            PlayerPrefs.SetInt("Unarmed", selectedStyle);
        }

        if (selectedStyle != 8 && selectedStyle != 9)
        {
            currentSpellOnAutocast = null;
            RemoveAutocast();
        }

        UpdateStylesWeaponChange();
    }

    void UpdateStylesWeaponChange()
    {
        bool[] setActive = new bool[styles.Length];
        for (int i = 0; i < styles.Length; i++)
        {
            styles[i].SetActive(true);
        }

        string category = "";
        if (WornEquipment.weapon != null)
        {
            category = WornEquipment.weapon.weaponCategory;
        }

        attBonus = 0; strBonus = 0; defBonus = 0; rangedBonus = 0; distanceBonus = 0; rapidBonus = 0; magicBonus = 0;

        if (WornEquipment.weapon == null)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = unarmedPunch;
            style1.GetComponentInChildren<Text>().text = "Punch";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = unarmedKick;
            style2.GetComponentInChildren<Text>().text = "Kick";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = unarmedBlock;
            style3.GetComponentInChildren<Text>().text = "Lunge";

            selectedStyle = PlayerPrefs.GetInt("Unarmed", 1);

            attackStyle = crushStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = aggressiveType;
            }
            else
            {
                attackType = defensiveType;
            }
        }



        else if (category == WornEquipment.slashSwordCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;
            setActive[3] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = swordChop;
            style1.GetComponentInChildren<Text>().text = "Chop";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = swordSlash;
            style2.GetComponentInChildren<Text>().text = "Slash";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = swordLunge;
            style3.GetComponentInChildren<Text>().text = "Lunge";

            style4.transform.GetChild(0).GetComponent<RawImage>().texture = swordBlock;
            style4.GetComponentInChildren<Text>().text = "Block";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.slashSwordCategory, 1);

            attackStyle = slashStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = aggressiveType;
            }
            else if (selectedStyle == 3)
            {
                attackStyle = stabStyle;
                attackType = controlledType;
            }
            else
            {
                attackType = defensiveType;
            }
        }
        else if (category == WornEquipment.stabSwordCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;
            setActive[3] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = swordLunge;
            style1.GetComponentInChildren<Text>().text = "Stab";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = swordChop;
            style2.GetComponentInChildren<Text>().text = "Lunge";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = swordSlash;
            style3.GetComponentInChildren<Text>().text = "Slash";

            style4.transform.GetChild(0).GetComponent<RawImage>().texture = swordBlock;
            style4.GetComponentInChildren<Text>().text = "Block";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.stabSwordCategory, 1);

            attackStyle = stabStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = aggressiveType;
            }
            else if (selectedStyle == 3)
            {
                attackType = aggressiveType;
                attackStyle = slashStyle;
            }
            else
            {
                attackType = defensiveType;
            }
        }



        else if (category == WornEquipment.bowCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = bowAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = bowRapid;
            style2.GetComponentInChildren<Text>().text = "Rapid";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = bowLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.bowCategory, 1);

            attackStyle = rangedStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = rapidType;
            }
            else
            {
                attackType = longrangeType;
            }
        }
        else if (category == WornEquipment.crossbowCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = cbowAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = cbowRapid;
            style2.GetComponentInChildren<Text>().text = "Rapid";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = cbowLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.crossbowCategory, 1);

            attackStyle = rangedStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = rapidType;
            }
            else
            {
                attackType = longrangeType;
            }
        }
        else if (category == WornEquipment.thrownCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = thrownAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = thrownRapid;
            style2.GetComponentInChildren<Text>().text = "Rapid";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = thrownLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.thrownCategory, 1);

            attackStyle = rangedStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackType = rapidType;
            }
            else
            {
                attackType = longrangeType;
            }
        }



        else if (category == WornEquipment.staffCategory || category == WornEquipment.bladedStaffCategory)
        {
            setActive[4] = true;
            setActive[5] = true;
            setActive[6] = true;
            setActive[7] = true;
            setActive[8] = true;

            if (category == WornEquipment.staffCategory)
            {
                selectedStyle = PlayerPrefs.GetInt(WornEquipment.staffCategory, 5);
            }
            else
            {
                selectedStyle = PlayerPrefs.GetInt(WornEquipment.bladedStaffCategory, 5);
            }

            if (selectedStyle == 8 || selectedStyle == 9)
            {
                if (currentSpellOnAutocast == null || currentSpellOnAutocast.available == false)
                {
                    RemoveAutocast();
                }
            }

            if (category == WornEquipment.staffCategory)
            {
                mageStyle5.GetComponentInChildren<Text>().text = "Bash";
                mageStyle6.GetComponentInChildren<Text>().text = "Pound";
                mageStyle7.GetComponentInChildren<Text>().text = "Focus";

                attackStyle = crushStyle;
                if (selectedStyle == 5)
                {
                    attackType = accurateType;
                }
                else if (selectedStyle == 6)
                {
                    attackType = aggressiveType;
                }
                else if (selectedStyle == 7)
                {
                    attackType = defensiveType;
                }
                else if (selectedStyle == 8)
                {
                    attackStyle = magicStyle;
                    attackType = defensiveType;
                }
                else if (selectedStyle == 9)
                {
                    attackStyle = magicStyle;
                    attackType = accurateType;
                }
            }
            else if (category == WornEquipment.bladedStaffCategory)
            {
                mageStyle5.GetComponentInChildren<Text>().text = "Jab";
                mageStyle6.GetComponentInChildren<Text>().text = "Swipe";
                mageStyle7.GetComponentInChildren<Text>().text = "Fend";

                if (selectedStyle == 5)
                {
                    attackStyle = stabStyle;
                    attackType = accurateType;
                }
                else if (selectedStyle == 6)
                {
                    attackStyle = slashStyle;
                    attackType = aggressiveType;
                }
                else if (selectedStyle == 7)
                {
                    attackStyle = crushStyle;
                    attackType = defensiveType;
                }
                else if (selectedStyle == 8)
                {
                    attackStyle = magicStyle;
                    attackType = defensiveType;
                }
                else if (selectedStyle == 9)
                {
                    attackStyle = magicStyle;
                    attackType = accurateType;
                }
            }

            WornEquipment.UpdateStats();
        }
        else if (category == WornEquipment.poweredStaffCategory)
        {
            setActive[0] = true;
            setActive[1] = true;
            setActive[2] = true;

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = thrownAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = thrownAccurate;
            style2.GetComponentInChildren<Text>().text = "Accurate";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = thrownLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt(WornEquipment.poweredStaffCategory, 1);

            attackStyle = magicStyle;
            if (selectedStyle != 3)
            {
                attackType = accurateType;
            }
            else
            {
                attackType = longrangeType;
            }
        }
        else
        {
            selectedStyle = 0;
        }

        UpdateBonuses();

        for (int i = 0; i < styles.Length; i++)
        {
            if (setActive[i] == false)
            {
                styles[i].SetActive(false);
            }
            else 
            {
                styles[i].GetComponent<AttackStyle>().styleName = styles[i].GetComponentInChildren<Text>().text;
                styles[i].GetComponent<Action>().menuTexts[0] = styles[i].GetComponentInChildren<Text>().text;
                if (styles[i].GetComponent<Action>().menuTexts[0] == "Spell")
                {
                    styles[i].GetComponent<Action>().menuTexts[0] = "Choose spell";
                }
            }
        }

        ChangeStyleColor(selectedStyle);
    }

    public void ChangeStyleColor(int selectedStyle)
    {
        foreach (GameObject style in styles)
        {
            if (style != null)
            {
                style.GetComponent<RawImage>().texture = styleOff;
            }
        }

        for (int i = 0; i < styles.Length; i++)
        {
            if (i + 1 == selectedStyle && styles[i] != null)
            {
                styles[i].GetComponent<RawImage>().texture = styleOn;
            }
        }
    }

    void UpdateBonuses()
    {
        string category = "Unarmed";
        if (WornEquipment.weapon != null)
        {
            category = WornEquipment.weapon.weaponCategory;
        }

        if (attackStyle == stabStyle || attackStyle == slashStyle || attackStyle == crushStyle)
        {
            if (attackType == accurateType)
            {
                attBonus = 3;
            }
            else if (attackType == aggressiveType)
            {
                strBonus = 3;
            }
            else if (attackType == defensiveType)
            {
                defBonus = 3;
            }
            else if (attackType == controlledType)
            {
                attBonus = 1;
                strBonus = 1;
                defBonus = 1;
            }
        }
        else if (attackStyle == rangedStyle)
        {
            if (attackType == accurateType)
            {
                rangedBonus = 3;
            }
            else if (attackType == rapidType)
            {
                rapidBonus = 1;
            }
            else if (attackType == longrangeType)
            {
                distanceBonus = 2;
                defBonus = 3;
            }
        }
        else if (attackStyle == magicStyle && category == "Powered Staff")
        {
            if (attackType == accurateType)
            {
                magicBonus = 3;
            }
            else if (attackType == longrangeType)
            {
                magicBonus = 1;
                distanceBonus = 2;
            }
        }
    }

    public void InitiateSelectAutocast(int styleNumber)
    {
        autocastSelectActive = true;
        if (styleNumber == 8)
        {
            selectingDefensive = true;
        }
        autocastPanel.transform.localPosition = Vector2.zero;
        mainPanel.transform.localPosition = Vector2.right * 1000;
    }
    public void SelectedAutocastSpell()
    {
        if (currentSpellOnAutocast != null)
        {
            styleChanged = true;
            normalCastImage.texture = currentSpellOnAutocast.spellImage.texture;
            normalCastImage.transform.localScale = Vector2.one * 1.4f;
            defensiveCastImage.texture = currentSpellOnAutocast.spellImage.texture;
            defensiveCastImage.transform.localScale = Vector2.one * 1.4f;
            if (selectingDefensive)
            {
                mageStyleDefensiveCast8.GetComponent<AttackStyle>().SelectStyle();
                selectedStyle = 8;
                PlayerPrefs.SetInt(WornEquipment.weapon.weaponCategory, 8);
            }
            else
            {
                mageStyleCast9.GetComponent<AttackStyle>().SelectStyle();
                selectedStyle = 9;
                PlayerPrefs.SetInt(WornEquipment.weapon.weaponCategory, 9);
            }
            UpdateStyleSelected();
        }
        else
        {
            defensiveCastImage.texture = defaultDefensiveCastTexture;
            defensiveCastImage.transform.localScale = Vector2.one;
            normalCastImage.texture = defaultNormalCastTexture;
            normalCastImage.transform.localScale = Vector2.one;
        }

        autocastSelectActive = false;
        selectingDefensive = false;
        autocastPanel.transform.localPosition = Vector2.right * 1000;
        mainPanel.transform.localPosition = Vector2.zero;
    }
    public void RemoveAutocast()
    {
        currentSpellOnAutocast = null;
        PlayerPrefs.SetInt(WornEquipment.staffCategory, 5);
        PlayerPrefs.SetInt(WornEquipment.bladedStaffCategory, 5);
        defensiveCastImage.texture = defaultDefensiveCastTexture;
        defensiveCastImage.transform.localScale = Vector2.one;
        normalCastImage.texture = defaultNormalCastTexture;
        normalCastImage.transform.localScale = Vector2.one;
        selectedStyle = 5;
        styleChanged = true;
        WornEquipment.UpdateStats();
    }
}
