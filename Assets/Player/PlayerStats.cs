using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int attack = 1;
    public int strength = 1;
    public int defence = 1;
    public int ranged = 1;
    public int magic = 1;
    public int hitpoints = 10;
    public int prayer = 1;
    public int agility = 1;

    public static int initialAttack;
    public static int initialStrength;
    public static int initialDefence;
    public static int initialRanged;
    public static int initialMagic;
    public static int initialHitpoints;
    public static int initialPrayer;

    public static int currentAttack;
    public static int currentStrength;
    public static int currentDefence;
    public static int currentRanged;
    public static int currentMagic;
    public static int currentHitpoints;
    public static int currentPrayer;

    public static float combatLevel;

    public static float truePrayer;
    float drainPerTick;
    public static bool prayersOnForATick = false;

    private void Start()
    {
        initialAttack = attack;
        initialStrength = strength;
        initialDefence = defence;
        initialRanged = ranged;
        initialMagic = magic;
        initialHitpoints = hitpoints;
        initialPrayer = prayer;

        currentAttack = attack;
        currentStrength = strength;
        currentDefence = defence;
        currentRanged = ranged;
        currentMagic = magic;
        currentHitpoints = hitpoints;
        currentPrayer = prayer;

        truePrayer = prayer * 100;
        TickManager.onTick += PrayerDrain;

        float baselvl = 0.25f * ((float) defence + hitpoints + (prayer * 0.5f));
        float meleelvl = (13f / 40f) * ((float) attack + strength);
        float rangelvl = (13f / 40f) * ((float) ranged * 3 / 2);
        float magelvl = (13f / 40f) * ((float) magic * 3 / 2);
        combatLevel = baselvl + Mathf.Max(meleelvl, rangelvl, magelvl);
    }

    void PrayerDrain()
    {
        if (truePrayer <= 0)
        {
            Prayer.DeactivatePrayers();
            truePrayer = 0;
            currentPrayer = (int)Mathf.Ceil(truePrayer / 100);
        }
        drainPerTick = Prayer.drainRate / (WornEquipment.prayer * 2 + 60);
        if (prayersOnForATick)
        {
            truePrayer -= drainPerTick * 100;
            currentPrayer = (int)Mathf.Ceil(truePrayer / 100);
        }
        if (drainPerTick > 0)
        {
            prayersOnForATick = true;
        }
    }
}
