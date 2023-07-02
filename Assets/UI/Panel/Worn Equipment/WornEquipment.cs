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

    public static List<Transform> slots = new List<Transform>();

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

    public static bool voidMelee;
    public static bool voidRange;
    public static bool voidMage;
    public static bool eliteVoid;
    public static bool slayerHelm;
    public static bool accumulator;
    public static bool assembler;
    public static float crystalBonus;
    public static bool crystalWeapon;
    public static bool lightBearer;
    public static bool recoil;
    public static bool justiciar;
    public static bool regenBrace;
    public static bool ely;

    public static bool diamondBoltsE;
    public static bool rubyBoltsE;

    Canvas canvas;
    public RectTransform statPanel;
    public Text statPanelText;
    public OpenCloseButton panelCloseButton;
    public OpenCloseButton panelOpenButton;
    float baseStatPanelPosition;

    public static string slashSwordCategory = "Slash Sword";
    public static string stabSwordCategory = "Stab Sword";
    public static string twoHandedSwordCategory = "2h Sword";
    public static string bulwarkCategory = "Bulwark";
    public static string bowCategory = "Bow";
    public static string crossbowCategory = "Crossbow";
    public static string thrownCategory = "Thrown";
    public static string chinchompaCategory = "Chinchompas";
    public static string staffCategory = "Staff";
    public static string bladedStaffCategory = "Bladed Staff";
    public static string poweredStaffCategory = "Powered Staff";

    public AudioClip[] equipSounds;
    [HideInInspector] public bool ignoreEquipSounds = true;

    public static WornEquipment instance;

    private IEnumerator Start()
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

        slots = new List<Transform>();
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

        instance = this;

        panelCloseButton.buttonClicked += StatsPanelActivate;
        panelOpenButton.buttonClicked += StatsPanelActivate;

        canvas = FindObjectOfType<Canvas>();

        ignoreEquipSounds = true;
        yield return new WaitForSeconds(1);
        ignoreEquipSounds = false;
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
                if (slot == ammoSlot && weapon != null && weapon.GetComponent<Equipment>().addAmmoRangedStats == false)
                {
                    continue;
                }

                Equipment item = slot.GetComponentInChildren<Equipment>();
                attackStab += item.attackStab;
                attackSlash += item.attackSlash;
                attackCrush += item.attackCrush;
                attackMagic += item.attackMagic;

                defenceStab += item.defenceStab;
                defenceSlash += item.defenceSlash;
                defenceCrush += item.defenceCrush;
                defenceRange += item.defenceRange;
                defenceMagic += item.defenceMagic;

                meleeStrength += item.meleeStrength;
                magicDamage += item.magicDamage;
                prayer += item.prayer;

                if (slot == ammoSlot && weapon != null && weapon.GetComponent<Equipment>().addAmmoRangedStats == false)
                {

                }
                else
                {
                    attackRange += item.attackRange;
                    rangedStrength += item.rangedStrength;
                }

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
                }
            }
            else if (slot == weaponSlot)
            {
                attackSpeed = 4;
                attackDistance = 1;
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
        if (cape != null && (cape.name == "Ava's assembler" || cape.name == "Masori assembler" || cape.name == "Ranging cape(t)"))
        {
            assembler = true;
        }
        else if (cape != null && cape.name == "Ava's accumulator")
        {
            accumulator = true;
        }

        if (weapon != null && weapon.GetComponent<SpecialEffects>() != null && weapon.GetComponent<SpecialEffects>().tumekensShadow)
        {
            attackMagic *= 3;
            magicDamage *= 3;
        }

        crystalBonus = 0;
        crystalWeapon = false;
        if (head != null && head.name == "Crystal helm")
        {
            crystalBonus += 0.05f;
        }
        if (body != null && body.name == "Crystal body")
        {
            crystalBonus += 0.15f;
        }
        if (leg != null && leg.name == "Crystal legs")
        {
            crystalBonus += 0.1f;
        }
        if (weapon != null && (weapon.name == "Bow of faerdhinen" || weapon.name == "Crystal bow"))
        {
            crystalWeapon = true;
        }

        diamondBoltsE = false;
        rubyBoltsE = false;
        if (weapon != null && weapon.weaponCategory == crossbowCategory)
        {
            if (ammo != null && ammo.name.ToLower().Contains("diamond"))
            {
                diamondBoltsE = true;
            }
            else if (ammo != null && ammo.name.ToLower().Contains("ruby"))
            {
                rubyBoltsE = true;
            }
        }

        lightBearer = false;
        if (ring != null && ring.name == "Lightbearer")
        {
            lightBearer = true;
        }

        recoil = false;
        if (ring != null && (ring.name.Contains("suffering") || ring.name.Contains("recoil")))
        {
            recoil = true;
        }

        justiciar = false;
        if (head != null && head.name == "Justiciar faceguard" && body != null && body.name == "Justiciar chestguard" && leg != null && leg.name == "Justiciar legguards")
        {
            justiciar = true;
        }

        regenBrace = false;
        if (glove != null && glove.name == "Regen bracelet")
        {
            regenBrace = true;
        }

        ely = false;
        if (shield != null && shield.name == "Elysian spirit shield")
        {
            ely = true;
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

    void StatsPanelActivate()
    {
        statPanel.gameObject.SetActive(!statPanel.gameObject.activeSelf);
    }

    public void ResetStatPanelPosition()
    {
        //Debug.Log(statPanel.position);
        //statPanel.position = new Vector2(-350 * canvas.scaleFactor, statPanel.position.y);
        //Debug.Log(statPanel.position);
    }

    public static void PlayEquipNoise()
    {
        Debug.Log(instance.ignoreEquipSounds);
        if (instance.ignoreEquipSounds)
        {
            return;
        }

        int rand = Random.Range(0, instance.equipSounds.Length);
        for (int i = 0; i < instance.equipSounds.Length; i++)
        {
            if (rand == i)
            {
                PlayerAudio.PlayClip(instance.equipSounds[i]);
            }
        }
    }
}
