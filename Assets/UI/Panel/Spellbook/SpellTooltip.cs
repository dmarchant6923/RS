using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellTooltip : MonoBehaviour
{
    [HideInInspector] public Spell currentSpell;

    RectTransform rt;

    public Text levelAndName;
    public Text description;
    public GameObject[] runeImages = new GameObject[4];
    public RectTransform runeImageParent;
    float runeImageInitialx;

    public Texture[] runeTextures = new Texture[9];

    float imageDist = 70;


    Vector3 onPositionLow;
    Vector3 onPositionHigh;
    Vector3 offPosition;

    [System.NonSerialized] public bool onAutocastSpellSelect = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        onPositionLow = rt.localPosition;
        onPositionHigh = onPositionLow * Vector2.down;
        offPosition = onPositionLow + Vector3.right * 1000;

        rt.localPosition = offPosition;

        runeImageInitialx = runeImageParent.localPosition.x;
    }

    void Update()
    {
        if (currentSpell == null && rt.localPosition != offPosition)
        {
            rt.localPosition = offPosition;
        }
    }

    public void ShowSpellInfo(Spell spell)
    {
        currentSpell = spell;
        if (spell.levelReq > 0)
        {
            levelAndName.text = "Level " + spell.levelReq + ": " + spell.name;
        }
        else
        {
            levelAndName.text = spell.name;
        }
        description.text = spell.description;
        string spellType = "combat";
        if (spell.teleportSpell)
        {
            spellType = "teleport";
        }
        else if (spell.utilitySpell)
        {
            spellType = "utility";
        }
        if (string.IsNullOrEmpty(spell.description))
        {
            description.text = "A level " + spell.levelReq + " " + spellType + " spell";
        }
        foreach (GameObject image in runeImages)
        {
            image.SetActive(false);
        }

        int count = 0;
        bool[] enoughRunes = new bool[4];
        for (int i = 0; i < spell.runes.Length; i++)
        {
            if (string.IsNullOrEmpty(spell.runes[i]) == false)
            {
                count++;
                runeImages[i].SetActive(true);
                foreach (Texture texture in runeTextures)
                {
                    if (texture.name.ToLower() == spell.runes[i].ToLower())
                    {
                        runeImages[i].GetComponent<RawImage>().texture = texture;
                        break;
                    }
                }
                int numberOfThisRune = 0;
                string str;
                if (Spellbook.runeSource != null && Spellbook.runeSource.infiniteSupply && 
                    (spell.runes[i].ToLower().Contains(Spellbook.runeSource.runeToSupply) || string.IsNullOrEmpty(Spellbook.runeSource.runeToSupply)))
                {
                    str = "*";
                    enoughRunes[i] = true;
                }
                else
                {
                    foreach (StackableItem item in Spellbook.runesInInventory)
                    {
                        if (item.name.ToLower() == spell.runes[i].ToLower())
                        {
                            numberOfThisRune = item.quantity;
                            if (item.quantity >= spell.quantities[i])
                            {
                                enoughRunes[i] = true;
                            }
                            break;
                        }
                    }
                    str = Tools.IntToShortString(numberOfThisRune);
                }


                runeImages[i].GetComponentInChildren<Text>().text = str + "/" + spell.quantities[i];
                runeImages[i].GetComponentInChildren<Text>().color = enoughRunes[i] ? Color.green : Color.red;
            }
        }

        //if (count == 0)
        //{
        //    rt.localPosition = offPosition;
        //    currentSpell = null;
        //}
        //else
        //{
            rt.localPosition = onPositionLow;
            if (spell.GetComponent<RectTransform>().localPosition.y < 0)
            {
                rt.localPosition = onPositionHigh;
            }
            runeImageParent.localPosition = new Vector3(runeImageInitialx, runeImageParent.localPosition.y);
            runeImageParent.localPosition += Vector3.left * 35 * (count - 1);
        //}
    }
}
