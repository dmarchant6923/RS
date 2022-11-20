using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WornEquipment : MonoBehaviour
{
    public Transform headTransform;
    public Transform capeTransform;
    public Transform neckTransform;
    public Transform ammoTransform;
    public Transform weaponTransform;
    public Transform bodyTransform;
    public Transform shieldTransform;
    public Transform legTransform;
    public Transform gloveTransform;
    public Transform bootTransform;
    public Transform ringTransform;

    public static Transform headSlot;
    public static Transform capeSlot;
    public static Transform neckSlot;
    public static Transform ammoSlot;
    public static Transform weaponSlot;
    public static Transform bodySlot;
    public static Transform shieldSlot;
    public static Transform legSlot;
    public static Transform gloveSlot;
    public static Transform bootSlot;
    public static Transform ringSlot;

    public static Equipment head;
    public static Equipment cape;
    public static Equipment neck;
    public static Equipment ammo;
    public static Equipment weapon;
    public static Equipment body;
    public static Equipment shield;
    public static Equipment leg;
    public static Equipment glove;
    public static Equipment boot;
    public static Equipment ring;

    static List<Transform> slots = new List<Transform>();

    public static int attackStab;
    public static int attackSlash;
    public static int attackCrush;
    public static int attackMagic;
    public static int attackRange;

    public static int defenceStab;
    public static int defenceSlash;
    public static int defenceCrush;
    public static int defenceMagic;
    public static int defenceRange;

    public static int meleeStrength;
    public static int rangedStrength;
    public static int magicDamage;
    public static int prayer;

    public static int attackSpeed;
    public static int attackDistance = 1;
    public static int specCost;

    public static bool voidMelee;
    public static bool voidRange;
    public static bool voidMage;
    public static bool eliteVoid;
    public static bool slayerHelm;
    public static bool accumulator;
    public static bool assembler;

    public Text statPanelText;

    public static string slashSwordCategory = "Slash Sword";
    public static string stabSwordCategory = "Stab Sword";
    public static string bowCategory = "Bow";
    public static string crossbowCategory = "Crossbow";
    public static string thrownCategory = "Thrown";
    public static string staffCategory = "Staff";
    public static string bladedStaffCategory = "Bladed Staff";
    public static string poweredStaffCategory = "Powered Staff";

    public static WornEquipment wornEquipment;

    private void Start()
    {
        headSlot = headTransform;
        capeSlot = capeTransform;
        neckSlot = neckTransform;
        ammoSlot = ammoTransform;
        weaponSlot = weaponTransform;
        bodySlot = bodyTransform;
        shieldSlot = shieldTransform;
        legSlot = legTransform;
        gloveSlot = gloveTransform;
        bootSlot = bootTransform;
        ringSlot = ringTransform;

        head = headSlot.GetComponentInChildren<Equipment>();
        cape = capeSlot.GetComponentInChildren<Equipment>();
        neck = neckSlot.GetComponentInChildren<Equipment>();
        ammo = ammoSlot.GetComponentInChildren<Equipment>();
        weapon = weaponSlot.GetComponentInChildren<Equipment>();
        body = bodySlot.GetComponentInChildren<Equipment>();
        shield = shieldSlot.GetComponentInChildren<Equipment>();
        leg = legSlot.GetComponentInChildren<Equipment>();
        glove = gloveSlot.GetComponentInChildren<Equipment>();
        boot = bootSlot.GetComponentInChildren<Equipment>();
        ring = ringSlot.GetComponentInChildren<Equipment>();

        slots.Add(headSlot);
        slots.Add(capeSlot);
        slots.Add(neckSlot);
        slots.Add(ammoSlot);
        slots.Add(weaponSlot);
        slots.Add(bodySlot);
        slots.Add(shieldSlot);
        slots.Add(legSlot);
        slots.Add(gloveSlot);
        slots.Add(bootSlot);
        slots.Add(ringSlot);

        UpdateStats();
        TickManager.onTick += UpdateText;
        Inventory.UpdateEquippedItems += UpdateStats;

        wornEquipment = this;
    }

    public static void UpdateStats()
    {
        head = headSlot.GetComponentInChildren<Equipment>();
        cape = capeSlot.GetComponentInChildren<Equipment>();
        neck = neckSlot.GetComponentInChildren<Equipment>();
        weapon = weaponSlot.GetComponentInChildren<Equipment>();
        ammo = ammoSlot.GetComponentInChildren<Equipment>();
        body = bodySlot.GetComponentInChildren<Equipment>();
        shield = shieldSlot.GetComponentInChildren<Equipment>();
        leg = legSlot.GetComponentInChildren<Equipment>();
        glove = gloveSlot.GetComponentInChildren<Equipment>();
        boot = bootSlot.GetComponentInChildren<Equipment>();
        ring = ringSlot.GetComponentInChildren<Equipment>();

        attackStab = 0; attackSlash = 0; attackCrush = 0; attackRange = 0; attackMagic = 0;
        defenceStab = 0; defenceSlash = 0; defenceCrush = 0; defenceRange = 0; defenceMagic = 0;
        meleeStrength = 0; rangedStrength = 0; magicDamage = 0; prayer = 0;

        foreach (Transform slot in slots)
        {
            if (slot.GetComponentInChildren<Equipment>() != null)
            {
                if (slot == ammoSlot && weapon != null && weapon.GetComponent<BlowPipe>() != null)
                {
                    continue;
                }

                Equipment item = slot.GetComponentInChildren<Equipment>();
                attackStab += item.attackStab;
                attackSlash += item.attackSlash;
                attackCrush += item.attackCrush;
                attackRange += item.attackRange;
                attackMagic += item.attackMagic;

                defenceStab += item.defenceStab;
                defenceSlash += item.defenceSlash;
                defenceCrush += item.defenceCrush;
                defenceRange += item.defenceRange;
                defenceMagic += item.defenceMagic;

                meleeStrength += item.meleeStrength;
                rangedStrength += item.rangedStrength;
                magicDamage += item.magicDamage;
                prayer += item.prayer;

                if (slot == weaponSlot)
                {
                    attackSpeed = item.attackSpeed;
                    attackDistance = item.attackDistance;
                    if (AttackStyles.currentSpellOnAutocast != null && AttackStyles.attackStyle == AttackStyles.magicStyle && weapon.weaponCategory != poweredStaffCategory)
                    {
                        attackDistance = 10;
                    }
                    if (item.GetComponent<RuneSource>() != null)
                    {
                        Spellbook.runeSource = item.GetComponent<RuneSource>();
                    }
                    else
                    {
                        Spellbook.runeSource = null;
                    }
                    specCost = item.specCost;
                }
            }
            else if (slot == weaponSlot)
            {
                attackSpeed = 4;
                attackDistance = 1;
                specCost = 0;
            }
        }

        EquipmentEffects();
    }

    public static void EquipmentEffects()
    {
        voidMelee = false;
        voidRange = false;
        voidMage = false;
        eliteVoid = false;
        bool fullVoid = true;
        if (glove == null || glove.name != "Void knight gloves")
        {
            fullVoid = false;
        }
        if (fullVoid && (leg == null || leg.name.ToLower().Contains("void")))
        {
            fullVoid = false;
        }
        if (fullVoid && (body == null || body.name.ToLower().Contains("void")))
        {
            fullVoid = false;
        }
        if (fullVoid && head != null && head.name.ToLower().Contains("void knight"))
        {
            if (leg.name == "Elite void robe" && body.name == "Elite void top")
            {
                eliteVoid = true;
            }

            if (head.name == "Void melee helm")
            {
                voidMelee = true;
            }
            if (head.name == "Void ranger helm")
            {
                voidRange = true;
            }
            if (head.name == "Void mage helm")
            {
                voidMage = true;
            }
        }

        slayerHelm = false;
        if (head != null && head.name.Contains("Slayer helmet"))
        {
            slayerHelm = true;
        }

        accumulator = false;
        assembler = false;
        if (cape != null && (cape.name == "Ava's assembler" || cape.name == "Masori assembler"))
        {
            assembler = true;
        }
        else if (cape != null && cape.name == "Ava's accumulator")
        {
            accumulator = true;
        }

        if (weapon != null && weapon.GetComponent<SpecialEffects>() != null && weapon.GetComponent<SpecialEffects>().TumekensShadow)
        {
            attackMagic *= 3;
            magicDamage *= 3;
        }
    }
    public void UpdateText()
    {
        statPanelText.text = "<b>Attack Bonus</b>\n" +
            "    Stab: " + attackStab + "\n" +
            "    Slash: " + attackSlash + "\n" +
            "    Crush: " + attackCrush + "\n" +
            "    Magic: " + attackMagic + "\n" +
            "    Range: " + attackRange + "\n\n" +
            "<b>Defence Bonus</b>\n" +
            "    Stab: " + defenceStab + "\n" +
            "    Slash: " + defenceSlash + "\n" +
            "    Crush: " + defenceCrush + "\n" +
            "    Magic: " + defenceMagic + "\n" +
            "    Range: " + defenceRange + "\n\n" +
            "<b>Other Bonuses</b>\n" +
            "    Melee Strength: " + meleeStrength + "\n" +
            "    Ranged Strength: " + rangedStrength + "\n" +
            "    Magic Damage: +" + magicDamage + "%\n" +
            "    prayer: " + prayer + "\n" + 
            "    Weapon Range: " + attackDistance;
    }
}