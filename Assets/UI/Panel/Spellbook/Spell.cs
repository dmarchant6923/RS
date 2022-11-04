using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int levelReq;
    public string description;

    public bool combatSpell;
    public bool teleportSpell;
    public bool utilitySpell;
    public int maxDamage;
    public int aoeSize = 1;

    public string rune1;
    public int quantity1;
    public string rune2;
    public int quantity2;
    public string rune3;
    public int quantity3;
    public string rune4;
    public int quantity4;

    [HideInInspector] public string[] runes = new string[4];
    [HideInInspector] public int[] quantities = new int[4];

    bool hasRune1;
    bool hasRune2;
    bool hasRune3;
    bool hasRune4;

    Action spellAction;
    bool available;
    bool isCastingSpell;

    SpellTooltip tooltip;

    RawImage maskImage;
    RawImage spellImage;
    RawImage mask;

    void Start()
    {
        if (combatSpell)
        {
            teleportSpell = false;
            utilitySpell = false;
        }
        else if (teleportSpell)
        {
            utilitySpell = false;
        }


        tooltip = FindObjectOfType<SpellTooltip>();
        spellAction = GetComponent<Action>();
        spellAction.menuTexts[0] = "Cast <color=lime>" + gameObject.name + "</color>";
        spellAction.clientAction0 += UseSpell;

        //spellImage = GetComponent<RawImage>();
        spellImage = transform.GetChild(1).GetComponent<RawImage>();
        spellImage.texture = GetComponent<RawImage>().texture;
        spellImage.gameObject.SetActive(true);
        Destroy(GetComponent<RawImage>());
        mask = transform.GetChild(0).GetComponent<RawImage>();
        mask.texture = spellImage.texture;


        runes[0] = rune1;
        runes[1] = rune2;
        runes[2] = rune3;
        runes[3] = rune4;

        quantities[0] = quantity1;
        quantities[1] = quantity2;
        quantities[2] = quantity3;
        quantities[3] = quantity4;

        Disable();
    }

    void UseSpell()
    {
        RightClickMenu.isCastingSpell = true;
        RightClickMenu.spellBeingCast = gameObject;
        mask.gameObject.SetActive(true);
        isCastingSpell = true;
    }

    public void CastSpell(GameObject target)
    {
        Debug.Log("cast spell on " + target.name);
    }

    void Update()
    {
        if (isCastingSpell && RightClickMenu.isCastingSpell == false)
        {
            isCastingSpell = false;
            mask.gameObject.SetActive(false);
        }
    }

    public void CheckRunes()
    {
        if (PlayerStats.currentMagic < levelReq)
        {
            if (available)
            {
                Disable();
            }
            return;
        }

        hasRune1 = string.IsNullOrEmpty(rune1);
        hasRune2 = string.IsNullOrEmpty(rune2);
        hasRune3 = string.IsNullOrEmpty(rune3);
        hasRune4 = string.IsNullOrEmpty(rune4);
        bool hasAll = false;
        foreach (StackableItem rune in Spellbook.runesInInventory)
        {
            if (hasRune1 == false && rune.name == rune1 && rune.quantity >= quantity1)
            {
                hasRune1 = true;
            }
            else if (hasRune2 == false && rune.name == rune2 && rune.quantity >= quantity2)
            {
                hasRune2 = true;
            }
            else if (hasRune3 == false && rune.name == rune3 && rune.quantity >= quantity3)
            {
                hasRune3 = true;
            }
            else if (hasRune4 == false && rune.name == rune4 && rune.quantity >= quantity4)
            {
                hasRune1 = true;
            }

            if (hasRune1 && hasRune2 && hasRune3 && hasRune4)
            {
                hasAll = true;
                break;
            }
        }

        if (hasAll && available == false)
        {
            Enable();
        }
        else if (hasAll == false && available)
        {
            Disable();
        }
    }

    void Enable()
    {
        available = true;
        spellImage.color = Color.white;
        if (GetComponentInChildren<Text>() != null)
        {
            GetComponentInChildren<Text>().color = Color.white;
        }
    }
    void Disable()
    {
        available = false;
        Color color = new Color(0.15f, 0.15f, 0.15f, 1);
        spellImage.color = color;
        if (GetComponentInChildren<Text>() != null)
        {
            GetComponentInChildren<Text>().color = color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowSpellInfo(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip.currentSpell == this)
        {
            tooltip.currentSpell = null;
        }
    }
}
