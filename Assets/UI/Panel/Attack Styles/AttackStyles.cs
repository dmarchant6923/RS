using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackStyles : MonoBehaviour
{
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

    public int selectedStyle = 1;
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

    string crushStyle = "Crush";
    string stabStyle = "Stab";
    string slashStyle = "Slash";
    string rangedStyle = "Ranged";
    string magicStyle = "Magic";

    string accurateType = "Accurate";
    string aggressiveType = "Aggressive";
    string defensiveType = "Defensive";
    string controlledType = "Controlled";
    string rapidType = "Rapid";
    string longrangeType = "Longrange";


    private IEnumerator Start()
    {
        styles[0] = style1; styles[1] = style2; styles[2] = style3; styles[3] = style4;
        styles[4] = mageStyle5; styles[5] = mageStyle6; styles[6] = mageStyle7; styles[7] = mageStyleDefensiveCast8; styles[8] = mageStyleCast9;

        Inventory.ReadEquippedItems += UpdateWeapon;
        TickManager.beforeTick += UpdateStyleSelected;
        combatLvlText.text = "Combat Lvl: " + PlayerStats.combatLevel.ToString("F2");

        yield return null;

        UpdateWeapon();
    }

    void UpdateWeapon()
    {

        bool hasSpec = false;
        if (WornEquipment.weapon != null)
        {
            weaponText.text = WornEquipment.weapon.name;
            categoryText.text = "Category: " + WornEquipment.weapon.weaponCategory;
            if (WornEquipment.weapon.hasSpec)
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

    void UpdateStylesWeaponChange()
    {
        foreach (GameObject style in styles)
        {
            style.SetActive(false);
        }

        string category = "";
        if (WornEquipment.weapon != null)
        {
            category = WornEquipment.weapon.weaponCategory;
        }

        attBonus = 0; strBonus = 0; defBonus = 0; rangedBonus = 0; distanceBonus = 0; rapidBonus = 0; magicBonus = 0;

        if (WornEquipment.weapon == null)
        {
            style1.SetActive(true);
            style2.SetActive(true);
            style3.SetActive(true);

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



        else if (category == "Slash Sword")
        {
            style1.SetActive(true);
            style2.SetActive(true);
            style3.SetActive(true);
            style4.SetActive(true);

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = swordChop;
            style1.GetComponentInChildren<Text>().text = "Chop";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = swordSlash;
            style2.GetComponentInChildren<Text>().text = "Slash";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = swordLunge;
            style3.GetComponentInChildren<Text>().text = "Lunge";

            style4.transform.GetChild(0).GetComponent<RawImage>().texture = swordBlock;
            style4.GetComponentInChildren<Text>().text = "Block";

            selectedStyle = PlayerPrefs.GetInt("Slash Sword", 1);

            attackStyle = slashStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackStyle = stabStyle;
                attackType = aggressiveType;
            }
            else if (selectedStyle == 3)
            {
                attackType = controlledType;
            }
            else
            {
                attackType = defensiveType;
            }
        }
        else if (category == "Stab Sword")
        {
            style1.SetActive(true);
            style2.SetActive(true);
            style3.SetActive(true);
            style4.SetActive(true);

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = swordLunge;
            style1.GetComponentInChildren<Text>().text = "Stab";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = swordChop;
            style2.GetComponentInChildren<Text>().text = "Lunge";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = swordSlash;
            style3.GetComponentInChildren<Text>().text = "Slash";

            style4.transform.GetChild(0).GetComponent<RawImage>().texture = swordBlock;
            style4.GetComponentInChildren<Text>().text = "Block";

            selectedStyle = PlayerPrefs.GetInt("Stab Sword", 1);

            attackStyle = stabStyle;
            if (selectedStyle == 1)
            {
                attackType = accurateType;
            }
            else if (selectedStyle == 2)
            {
                attackStyle = slashStyle;
                attackType = aggressiveType;
            }
            else if (selectedStyle == 3)
            {
                attackType = aggressiveType;
            }
            else
            {
                attackType = defensiveType;
            }
        }



        else if (category == "Bow")
        {
            style1.SetActive(true);
            style2.SetActive(true);
            style3.SetActive(true);

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = bowAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = bowRapid;
            style2.GetComponentInChildren<Text>().text = "Rapid";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = bowLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt("Bow", 1);

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



        else if (category == "Staff" || category == "Bladed Staff")
        {
            mageStyle5.SetActive(true);
            mageStyle6.SetActive(true);
            mageStyle7.SetActive(true);
            mageStyleDefensiveCast8.SetActive(true);
            mageStyleCast9.SetActive(true);

            if (category == "Staff")
            {
                mageStyle5.GetComponentInChildren<Text>().text = "Bash";
                mageStyle6.GetComponentInChildren<Text>().text = "Pound";
                mageStyle7.GetComponentInChildren<Text>().text = "Focus";
                selectedStyle = PlayerPrefs.GetInt("Staff", 5);

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
                else if (selectedStyle == 8)
                {
                    attackStyle = magicStyle;
                    attackType = accurateType;
                }
            }
            else if (category == "Bladed Staff")
            {
                mageStyle5.GetComponentInChildren<Text>().text = "Jab";
                mageStyle6.GetComponentInChildren<Text>().text = "Swipe";
                mageStyle7.GetComponentInChildren<Text>().text = "Fend";
                selectedStyle = PlayerPrefs.GetInt("Bladed Staff", 5);

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
                else if (selectedStyle == 8)
                {
                    attackStyle = magicStyle;
                    attackType = accurateType;
                }
            }
        }
        else if (category == "Powered Staff")
        {
            style1.SetActive(true);
            style2.SetActive(true);
            style3.SetActive(true);

            style1.transform.GetChild(0).GetComponent<RawImage>().texture = thrownAccurate;
            style1.GetComponentInChildren<Text>().text = "Accurate";

            style2.transform.GetChild(0).GetComponent<RawImage>().texture = thrownAccurate;
            style2.GetComponentInChildren<Text>().text = "Accurate";

            style3.transform.GetChild(0).GetComponent<RawImage>().texture = thrownLongrange;
            style3.GetComponentInChildren<Text>().text = "Longrange";

            selectedStyle = PlayerPrefs.GetInt("Powered Staff", 1);

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

        foreach (GameObject style in styles)
        {
            if (style.activeSelf)
            {
                style.GetComponent<AttackStyle>().styleName = style.GetComponentInChildren<Text>().text;
                style.GetComponent<Action>().menuTexts[0] = style.GetComponentInChildren<Text>().text;
                if (style.GetComponent<Action>().menuTexts[0] == "Spell")
                {
                    style.GetComponent<Action>().menuTexts[0] = "Choose spell";
                }
            }
        }

        ChangeStyleColor(selectedStyle);
    }

    public void ChangeStyleColor(int selectedStyle)
    {
        foreach (GameObject style in styles)
        {
            style.GetComponent<RawImage>().texture = styleOff;
        }

        for (int i = 0; i < styles.Length; i++)
        {
            if (i + 1 == selectedStyle)
            {
                styles[i].GetComponent<RawImage>().texture = styleOn;
            }
        }
    }

    void UpdateStyleSelected()
    {
        ChangeStyleColor(selectedStyle);
        if (styleChanged == false)
        {
            return;
        }
        styleChanged = false;

        UpdateBonuses();

        if (WornEquipment.weapon != null)
        {
            PlayerPrefs.SetInt(WornEquipment.weapon.weaponCategory, selectedStyle);
        }
        else
        {
            PlayerPrefs.SetInt("Unarmed", selectedStyle);
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
                distanceBonus = 1;
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
                distanceBonus = 1;
            }
        }
    }
}
