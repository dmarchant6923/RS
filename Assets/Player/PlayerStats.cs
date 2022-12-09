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

    public static int initialAttack;
    public static int initialStrength;
    public static int initialDefence;
    public static int initialRanged;
    public static int initialMagic;
    public static int initialHitpoints;
    public static int initialPrayer;

    public static int totalLevel;

    public static int currentAttack;
    public static int currentStrength;
    public static int currentDefence;
    public static int currentRanged;
    public static int currentMagic;
    public static int currentHitpoints;
    public static int currentPrayer;

    public static int[] setInitialStats = new int[7];

    public static float combatLevel;

    public static float truePrayer;
    float drainPerTick;
    public static bool prayersOnForATick = false;

    bool timerActive;
    int initialTickTimer = 100;
    int tickTimer;
    float timer;
    bool preserve = false;

    public static int divineTicks = 500;

    public static PlayerStats instance;

    static int divineAttackTimer = 0;
    static int divineStrengthTimer = 0;
    static int divineDefenseTimer = 0;
    static int divineRangedTimer = 0;
    static int divineMagicTimer = 0;

    public static int drinkDelay = 0;
    public static int foodDelay = 0;
    public static int karambwanDelayLol = 0;

    //static bool ignoreAttackDecrease = false;
    //static bool ignoreStrengthDecrease = false;
    //static bool ignoreDefenseDecrease = false;
    //static bool ignoreRangeDecrease = false;
    //static bool ignoreMagicDecrease = false;

    public delegate void ChangeStats();
    public static event ChangeStats newStats;

    public static int imbuedHeartRechargeTicks = 700;
    public static bool imbuedHeartCharged = true;
    public static int imbuedHeartTicks = 0;

    private IEnumerator Start()
    {
        instance = this;

        if (setInitialStats[0] > 0)
        {
            initialAttack = setInitialStats[0];
            initialStrength = setInitialStats[1];
            initialDefence = setInitialStats[2];
            initialRanged = setInitialStats[3];
            initialMagic = setInitialStats[4];
            initialHitpoints = setInitialStats[5];
            initialPrayer = setInitialStats[6];
            //setInitialStats = new int[7];
        }
        else
        {
            initialAttack = attack;
            initialStrength = strength;
            initialDefence = defence;
            initialRanged = ranged;
            initialMagic = magic;
            initialHitpoints = hitpoints;
            initialPrayer = prayer;
        }

        currentAttack = initialAttack;
        currentStrength = initialStrength;
        currentDefence = initialDefence;
        currentRanged = initialRanged;
        currentMagic = initialMagic;
        currentHitpoints = initialHitpoints;
        currentPrayer = initialPrayer;

        preserve = false;

        truePrayer = (float) initialPrayer * 100;
        TickManager.beforeTick += BoostTimer;
        TickManager.onTick += PrayerDrain;
        TickManager.afterTick += AfterTick;
        Player.player.tookDamage += Redemption;
        prayersOnForATick = false;

        float baselvl = 0.25f * ((float) defence + hitpoints + (prayer * 0.5f));
        float meleelvl = (13f / 40f) * ((float) attack + strength);
        float rangelvl = (13f / 40f) * ((float) ranged * 3 / 2);
        float magelvl = (13f / 40f) * ((float) magic * 3 / 2);
        combatLevel = baselvl + Mathf.Max(meleelvl, rangelvl, magelvl);

        yield return null;

        BuffBar.instance.UpdateBaseStats();

        SetTotalLevel();

        imbuedHeartTicks = 0;
    }

    public static void SetTotalLevel()
    {
        totalLevel = 0;
        totalLevel += initialAttack;
        totalLevel += initialStrength;
        totalLevel += initialDefence;
        totalLevel += initialRanged;
        totalLevel += initialMagic;
        totalLevel += initialHitpoints;
        totalLevel += initialPrayer;
    }

    void BoostTimer()
    {
        if (timerActive)
        {
            if (tickTimer <= 0)
            {
                StatRestoreTick();
                tickTimer = initialTickTimer;
                timer = (float)initialTickTimer * TickManager.maxTickTime;
            }
            tickTimer--;
            if (Prayer.preserve && preserve == false && tickTimer > 25)
            {
                preserve = true;
                initialTickTimer = 150;
                tickTimer += 50;
                timer += 30;
            }
            if (Prayer.preserve == false && preserve)
            {
                preserve = false;
                initialTickTimer = 100;
                tickTimer -= 50;
                timer -= 30;
            }
        }
        else
        {
            BuffBar.instance.DisableTimer();
            timerActive = false;
            tickTimer = initialTickTimer;
            timer = (float)initialTickTimer * TickManager.maxTickTime;
        }

        bool update = false;
        if (divineAttackTimer > 0)
        {
            divineAttackTimer--;
            if (divineAttackTimer == 0 && currentAttack > initialAttack)
            {
                currentAttack = initialAttack;
                update = true;
            }
        }
        if (divineStrengthTimer > 0)
        {
            divineStrengthTimer--;
            if (divineStrengthTimer == 0 && currentStrength > initialStrength)
            {
                currentStrength = initialStrength;
                update = true;
            }
        }
        if (divineDefenseTimer > 0)
        {
            divineDefenseTimer--;
            if (divineDefenseTimer == 0 && currentDefence > initialDefence)
            {
                currentDefence = initialDefence;
                update = true;
            }
        }
        if (divineRangedTimer > 0)
        {
            divineRangedTimer--;
            if (divineRangedTimer == 0 && currentRanged > initialRanged)
            {
                currentRanged = initialRanged;
                update = true;
            }
        }
        if (divineMagicTimer > 0)
        {
            divineMagicTimer--;
            if (divineMagicTimer == 0 && currentMagic > initialMagic)
            {
                currentMagic = initialMagic;
                update = true;
            }
        }

        if (update)
        {
            BuffBar.instance.UpdateStats();
        }


        if (drinkDelay > 0)
        {
            drinkDelay--;
        }
    }

    private void Update()
    {
        if (timerActive)
        {
            if (Mathf.Floor(timer) > Mathf.Floor(timer - Time.deltaTime) && timer - Time.deltaTime > 0)
            {
                BuffBar.instance.UpdateTimer(timer - Time.deltaTime);
            }
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
        }
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

    void AfterTick()
    {
        if (currentHitpoints <= 0)
        {
            currentHitpoints = 0;
        }

        if (imbuedHeartTicks > 0)
        {
            imbuedHeartTicks--;
        }
        else
        {
            imbuedHeartCharged = true;
        }
    }

    void Redemption(int damage)
    {
        if (Prayer.redemption && currentHitpoints > 0 && currentHitpoints <= (float)initialHitpoints / 10)
        {
            truePrayer = 0;
            currentPrayer = 0;
            Prayer.DeactivatePrayers();
            Player.player.ClearDamageQueue();
            PlayerHeal(Mathf.FloorToInt((float)initialPrayer / 4));
        }
    }

    public static void PlayerHeal(int amount)
    {
        PlayerHeal(amount, 0);
    }

    public static void PlayerHeal(int amount, int overHeal)
    {
        if (currentHitpoints >= initialHitpoints + overHeal)
        {

        }
        else
        {
            currentHitpoints = Mathf.Min(currentHitpoints + amount, initialHitpoints + overHeal);
        }
    }

    public static void PotionStatBoost(PotionStatBuff potion)
    {
        bool showTimer = false;
        if (potion.attack)
        {
            showTimer = true;
            currentAttack = potion.CalcBoost(initialAttack, currentAttack);
        }
        if (potion.strength)
        {
            showTimer = true;
            currentStrength = potion.CalcBoost(initialStrength, currentStrength);
        }
        if (potion.defense)
        {
            showTimer = true;
            currentDefence = potion.CalcBoost(initialDefence, currentDefence);
        }
        if (potion.range)
        {
            showTimer = true;
            currentRanged = potion.CalcBoost(initialRanged, currentRanged);
        }
        if (potion.mage)
        {
            showTimer = true;
            currentMagic = potion.CalcBoost(initialMagic, currentMagic);
        }
        if (potion.prayer)
        {
            showTimer = true;
            truePrayer = Mathf.Min(truePrayer + (potion.CalcBoost(initialPrayer, currentPrayer) - currentPrayer) * 100, initialPrayer * 100);
            currentPrayer = (int)Mathf.Ceil(truePrayer / 100);

        }
        if (potion.hitpoints)
        {
            showTimer = true;
            currentHitpoints = potion.CalcBoost(initialHitpoints, currentHitpoints);
        }

        if (potion.divine)
        {
            NewDivineTimer(potion);
        }
        else if (showTimer && instance.timerActive == false)
        {
            instance.timerActive = true;
            instance.tickTimer = instance.initialTickTimer;
            instance.timer = (float)instance.initialTickTimer * TickManager.maxTickTime;
        }

        drinkDelay = 3;
        foodDelay = 3;

        BuffBar.instance.UpdateStats();
    }

    public static void StartSkillTimer()
    {
        if (instance.timerActive == false)
        {
            instance.timerActive = true;
            instance.tickTimer = instance.initialTickTimer;
            instance.timer = (float)instance.initialTickTimer * TickManager.maxTickTime;
        }
    }

    public static void InvigorateImbuedHeart()
    {
        StartSkillTimer();
        imbuedHeartTicks = imbuedHeartRechargeTicks;
        imbuedHeartCharged = false;
        BuffBar.instance.UpdateStats();
    }

    void StatRestoreTick()
    {
        bool keepTimerActive = false;
        if (currentAttack != initialAttack && divineAttackTimer == 0)
        {
            currentAttack += (int)Mathf.Sign(initialAttack - currentAttack);
            keepTimerActive = true;
        }
        if (currentStrength != initialStrength && divineStrengthTimer == 0)
        {
            currentStrength += (int)Mathf.Sign(initialStrength - currentStrength);
            keepTimerActive = true;
        }
        if (currentDefence != initialDefence && divineDefenseTimer == 0)
        {
            currentDefence += (int)Mathf.Sign(initialDefence - currentDefence);
            keepTimerActive = true;
        }
        if (currentRanged != initialRanged && divineRangedTimer == 0)
        {
            currentRanged += (int)Mathf.Sign(initialRanged - currentRanged);
            keepTimerActive = true;
        }
        if (currentMagic != initialMagic && divineMagicTimer == 0)
        {
            currentMagic += (int)Mathf.Sign(initialMagic - currentMagic);
            keepTimerActive = true;
        }
        if (currentPrayer > initialPrayer)
        {
            truePrayer -= 100;
            currentPrayer = (int)Mathf.Ceil(truePrayer / 100);
            keepTimerActive = true;
        }

        if (keepTimerActive == false)
        {
            timerActive = false;
        }

        BuffBar.instance.UpdateStats();
    }

    public static void NewDivineTimer(PotionStatBuff buffScript)
    {
        divineAttackTimer = buffScript.attack ? divineTicks : divineAttackTimer;
        divineStrengthTimer = buffScript.strength ? divineTicks : divineStrengthTimer;
        divineDefenseTimer = buffScript.defense ? divineTicks : divineDefenseTimer;
        divineRangedTimer = buffScript.range ? divineTicks : divineRangedTimer;
        divineMagicTimer = buffScript.mage ? divineTicks : divineMagicTimer;
        Texture texture = buffScript.potionScript.CreatePotionTexture(4);
        string name = buffScript.name;
        string warningText = "<color=red>Your divine potion effect is about to expire.</color>";
        string endText = "<color=red>The effects of the divine potion have worn off.</color>";

        BuffBar.instance.CreateExtraTimer(texture, (float)divineTicks * TickManager.maxTickTime, name, warningText, endText);
    }

    public static void ReinitializeStats()
    {
        currentAttack = initialAttack;
        currentStrength = initialStrength;
        currentDefence = initialDefence;
        currentRanged = initialRanged;
        currentMagic = initialMagic;
        currentHitpoints = initialHitpoints;
        currentPrayer = initialPrayer;
        truePrayer = (float)initialPrayer * 100;

        newStats?.Invoke();
    }

    private void OnDestroy()
    {
        if (newStats != null)
        {
            foreach (var d in newStats.GetInvocationList())
            {
                newStats -= d as ChangeStats;
            }
        }
    }
}
