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

    public bool freeze;
    public int freezeLength;
    public bool leech;
    public int leechPercent;
    public bool poison;
    public int poisonDamage;
    public bool attackDebuff;
    public int debuffPercent;

    public Color projectileColor;
    public AudioClip castSound;
    public AudioClip damageSound;

    [HideInInspector] public string[] runes = new string[4];
    [HideInInspector] public int[] quantities = new int[4];

    bool hasRune1;
    bool hasRune2;
    bool hasRune3;
    bool hasRune4;

    Action spellAction;
    [System.NonSerialized] public bool available;
    bool isCastingSpell;

    SpellTooltip tooltip;

    RawImage maskImage;
    [HideInInspector] public RawImage spellImage;
    RawImage mask;

    [System.NonSerialized] public bool onAutocastSelectPanel = false;
    [System.NonSerialized] public ChooseSpell chooseSpellScript;
    [HideInInspector] public AttackStyles attackStyleScript;

    IEnumerator Start()
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


        spellAction = GetComponent<Action>();
        if (onAutocastSelectPanel == false)
        {
            spellAction.menuTexts[0] = "Cast <color=lime>" + gameObject.name + "</color>";
            spellAction.clientAction0 += UseSpell;
        }
        else
        {
            spellAction.menuTexts[0] = gameObject.name;
            spellAction.serverAction0 += SelectAutocast;
        }

        //spellImage = GetComponent<RawImage>();
        spellImage = transform.GetChild(1).GetComponent<RawImage>();
        spellImage.texture = GetComponent<RawImage>().texture;
        spellImage.gameObject.SetActive(true);
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

        yield return null;
        yield return null;
        tooltip = transform.parent.parent.GetComponentInChildren<SpellTooltip>();
        Destroy(GetComponent<RawImage>());
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
        if (target.GetComponent<Enemy>() == null)
        {
            GameLog.Log("You can't attack this.");
            return;
        }
        Player.player.attackUsingSpell = true;
        Player.player.spellBeingUsed = this;
        target.GetComponent<Action>().PickAction(0);
    }

    void Update()
    {
        if (isCastingSpell && RightClickMenu.isCastingSpell == false)
        {
            isCastingSpell = false;
            mask.gameObject.SetActive(false);
        }

        //Debug.Log(spellAction.menuTexts[0]);
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

        if (Spellbook.runeSource != null && Spellbook.runeSource.infiniteSupply)
        {
            for (int i = 0; i < runes.Length; i++)
            {
                if (runes[i].ToLower().Contains(Spellbook.runeSource.runeToSupply) || string.IsNullOrEmpty(Spellbook.runeSource.runeToSupply))
                {
                    if (i == 0)
                    {
                        hasRune1 = true;
                    }
                    else if (i == 1)
                    {
                        hasRune2 = true;
                    }
                    else if (i == 2)
                    {
                        hasRune3 = true;
                    }
                    else
                    {
                        hasRune4 = true;
                    }
                }
            }
        }
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
                hasRune4 = true;
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
    public void SpellNotAvailable()
    {
        if (available == false)
        {
            string missingRune = "";
            if (hasRune1 == false)
            {
                missingRune = rune1;
            }
            else if (hasRune2 == false)
            {
                missingRune = rune2;
            }
            else if (hasRune3 == false)
            {
                missingRune = rune3;
            }
            else if (hasRune4 == false)
            {
                missingRune = rune4;
            }

            if (string.IsNullOrEmpty(missingRune) == false)
            {
                GameLog.Log("You do not have enough " + missingRune + "s to cast this spell.");
            }
        }
    }

    void SelectAutocast()
    {
        if (AttackStyles.autocastSelectActive == false)
        {
            return;
        }

        if (available == false)
        {
            SpellNotAvailable();
        }
        else
        {
            foreach (Spell spell in Spellbook.spells)
            {
                if (spell.name == gameObject.name)
                {
                    AttackStyles.currentSpellOnAutocast = spell;
                }
            }
        }

        attackStyleScript.SelectedAutocastSpell();
    }
    public void UseRunes()
    {
        if (available == false)
        {
            Debug.Log("ERROR: got to subtract runes while spell was not available somehow");
            return;
        }

        for (int i = 0; i < runes.Length; i++)
        {
            foreach (StackableItem rune in Spellbook.runesInInventory)
            {
                if (runes[i] == rune.name)
                {
                    if (Spellbook.runeSource != null)
                    {
                        if (Spellbook.runeSource.infiniteSupply && (rune.name.ToLower().Contains(Spellbook.runeSource.runeToSupply) || string.IsNullOrEmpty(Spellbook.runeSource.runeToSupply)))
                        {

                        }
                        else if (Spellbook.runeSource.saveRunes && (Spellbook.runeSource.runeToSave == rune.name || string.IsNullOrEmpty(Spellbook.runeSource.runeToSave)))
                        {
                            float rand = Random.Range(0f, 1f);
                            if (rand > Spellbook.runeSource.saveChance)
                            {
                                rune.AddToQuantity(-quantities[i]);
                            }
                        }
                        else
                        {
                            rune.AddToQuantity(-quantities[i]);
                        }
                    }
                    else
                    {
                        rune.AddToQuantity(-quantities[i]);
                    }
                }
            }
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
        if (tooltip != null)
        {
            tooltip.ShowSpellInfo(this);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null && tooltip.currentSpell == this)
        {
            tooltip.currentSpell = null;
        }
    }
}
