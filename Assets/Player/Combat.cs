using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public bool onPlayer = false;

    Player playerScript;
    Pathfinder pathFinder;
    [HideInInspector] public Enemy targetEnemy;
    bool specialEffects;
    SpecialEffects effects;
    [HideInInspector] Spell playerSpell;

    [HideInInspector] public int attackCooldown = 0;
    [HideInInspector] public int minCoolDown = -7;

    public GameObject projectile;
    public float projectileSize = 1;

    public bool debug;
    LineRenderer line;
    public GameObject LOSIntersection;
    List<GameObject> intersections = new List<GameObject>();
    public GameObject checkedTileMarker;
    List<GameObject> checkedTileMarkers = new List<GameObject>();

    bool melee;
    bool mage;
    bool range;

    string style;

    bool offensivePrayerOn = false;

    [HideInInspector] public bool projectileSpawnsImmediately = false;

    public GameObject attackEffect;

    [HideInInspector] public float diamondBoltProcChance = 0.1f;
    [HideInInspector] public float rubyBoltProcChance = 0.11f;
    float realDiamondBoltChance;
    float realRubyBoltChance;

    [HideInInspector] public bool useSpec;
    SpecialAttack specScript;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        pathFinder = FindObjectOfType<Pathfinder>();

        TickManager.beforeTick += BeforeTick;

        specialEffects = false;
        effects = null;

        if (debug)
        {
            line = GetComponent<LineRenderer>();
        }

        if (playerScript.kandarinHard)
        {
            diamondBoltProcChance *= 1.1f;
            rubyBoltProcChance *= 1.1f;
        }

        realDiamondBoltChance = diamondBoltProcChance;
        realRubyBoltChance = rubyBoltProcChance;
    }
    public void PlayerAttack()
    {
        if (onPlayer == false)
        {
            return;
        }

        playerScript.attackThisTick = false;

        if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
        {
            if (WornEquipment.weapon.addAmmoRangedStats)
            {
                if (WornEquipment.ammo == null)
                {
                    Debug.Log("There is no ammo left in your quiver.");
                    playerScript.RemoveFocus();
                    return;
                }
                if (WornEquipment.weapon.weaponCategory == WornEquipment.bowCategory && WornEquipment.ammo.ammoCategory.ToLower().Contains("arrow") == false)
                {
                    Debug.Log("You can't use that ammo with your bow.");
                    playerScript.RemoveFocus();
                    return;
                }
                if (WornEquipment.weapon.weaponCategory == WornEquipment.crossbowCategory && WornEquipment.ammo.ammoCategory.ToLower().Contains("bolt") == false)
                {
                    Debug.Log("You can't use that ammo with your crossbow.");
                    playerScript.RemoveFocus();
                    return;
                }
            }
            else
            {
                if (WornEquipment.weapon.GetComponent<BlowPipe>() != null && WornEquipment.weapon.GetComponent<BlowPipe>().numberLoaded <= 0)
                {
                    Debug.Log("Your weapon has no ammo.");
                    return;
                }
            }
        }

        if (AttackStyles.attackStyle == AttackStyles.magicStyle && WornEquipment.weapon.weaponCategory == WornEquipment.poweredStaffCategory)
        {
            if (WornEquipment.weapon.GetComponent<ChargeItem>().charges <= 0)
            {
                Debug.Log("The " + WornEquipment.weapon.name + " has no charges left.");
                playerScript.RemoveFocus();
                return;
            }
        }

        Prayer.UpdatePrayerBonuses();
        SpecBar.instance.CheckSpec();


        if (attackCooldown <= 0)
        {
            playerScript.attackThisTick = true;
            attackCooldown = WornEquipment.attackSpeed;
            if (AttackStyles.attackStyle == AttackStyles.rangedStyle && AttackStyles.attackType == AttackStyles.rapidType)
            {
                attackCooldown--;
            }
            HealthHUD.Activate(Player.targetedNPC.GetComponent<Enemy>());



            
            specialEffects = false;
            effects = null;
            if (WornEquipment.weapon != null && WornEquipment.weapon.GetComponent<SpecialEffects>() != null)
            {
                specialEffects = true;
                effects = WornEquipment.weapon.GetComponent<SpecialEffects>();
            }
            if (useSpec && WornEquipment.weapon.spec == null)
            {
                specScript = null;
                useSpec = false;
            }
            else if (useSpec)
            {
                specScript = WornEquipment.weapon.spec;
            }



            float maxAttRoll = PlayerAttackRoll();
            float maxDefRoll = EnemyDefenseRoll(Player.targetedNPC.GetComponent<Enemy>(), AttackStyles.attackStyle);
            bool success = SuccessfulAttack(maxAttRoll, maxDefRoll);
            float maxHit = PlayerMaxHit();

            int hitRoll = 0;
            if (success)
            {
                hitRoll = Random.Range(0, (int)maxHit + 1);
            }


            if (WornEquipment.diamondBoltsE)
            {
                realDiamondBoltChance = diamondBoltProcChance;
                if (useSpec)
                {
                    realDiamondBoltChance = specScript.EnchantedBoltChance(realDiamondBoltChance, true);
                }
                float rand = Random.Range(0f, 1f);
                if (rand <= realDiamondBoltChance) //0.11f
                {
                    success = true;
                    float mult = 1.15f;
                    if (specialEffects && effects.zaryteCrossbow)
                    {
                        mult = 1.25f;
                    }
                    hitRoll = Random.Range(0, Mathf.FloorToInt(maxHit * mult) + 1);
                    GameObject newEffect = Instantiate(attackEffect, Player.player.trueTile, Quaternion.identity);
                    newEffect.GetComponent<Explosion>().followObject = playerScript.gameObject;
                    newEffect.GetComponent<SpriteRenderer>().color = Color.white;
                    newEffect.GetComponent<Explosion>().decaySpeed = 0.4f;
                    newEffect.GetComponent<Explosion>().rotate = true;

                }
            }
            if (WornEquipment.rubyBoltsE)
            {
                realRubyBoltChance = rubyBoltProcChance;
                if (useSpec)
                {
                    realRubyBoltChance = specScript.EnchantedBoltChance(realRubyBoltChance, success);
                }
                float rand = Random.Range(0f, 1f);
                if (rand <= realRubyBoltChance) //0.121f
                {
                    success = true;
                    float mult = 0.2f;
                    if (specialEffects && effects.zaryteCrossbow)
                    {
                        mult = 0.22f;
                    }
                    hitRoll = (int)Mathf.Min(100, Mathf.Floor((float)Player.targetedNPC.GetComponent<Enemy>().hitpoints * mult));
                    Player.player.AddToDamageQueue((int) Mathf.Floor((float)PlayerStats.currentHitpoints * 0.1f), 0, null);
                    GameObject newEffect = Instantiate(attackEffect, Player.player.trueTile, Quaternion.identity);
                    newEffect.GetComponent<Explosion>().followObject = playerScript.gameObject;
                    newEffect.GetComponent<SpriteRenderer>().color = Color.red;
                    newEffect.GetComponent<Explosion>().decaySpeed = 0.4f;
                    newEffect.GetComponent<Explosion>().rotate = true;
                }
            }

            if (hitRoll > 0)
            {
                XPDrop.CombatXPDrop(AttackStyles.attackStyle, AttackStyles.attackType, hitRoll, offensivePrayerOn);
            }

            int delay = CalculateDelay();
            if (useSpec && specScript.customProjectile != null)
            {
                SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, specScript.customProjectileColor, specScript.customProjectile);
            }
            else if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
            {
                if (WornEquipment.weapon.addAmmoRangedStats && WornEquipment.weapon.overrideProjectileColor.a == 0)
                {
                    SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, WornEquipment.ammo.GetComponent<StackableItem>().projectileColor, WornEquipment.weapon.weaponCategory);
                }
                else
                {
                    SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, WornEquipment.weapon.overrideProjectileColor, WornEquipment.weapon.weaponCategory);
                }
            }
            else if (AttackStyles.attackStyle == AttackStyles.magicStyle)
            {
                SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, WornEquipment.weapon.GetComponent<PoweredStaff>().projectileColor, WornEquipment.weapon.weaponCategory);
                WornEquipment.weapon.GetComponent<ChargeItem>().UseCharge();
            }

            if (useSpec && specScript.leech)
            {
                PlayerStats.PlayerHeal(Mathf.FloorToInt((float)hitRoll * specScript.leechPercent));
            }




            if (AttackStyles.attackStyle == AttackStyles.magicStyle && success == false)
            {

            }
            else
            {
                Player.targetedNPC.GetComponent<Enemy>().AddToDamageQueue(hitRoll, delay, true, (int)maxHit);
            }


            if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
            {
                if (WornEquipment.weapon.addAmmoRangedStats)
                {
                    WornEquipment.ammo.GetComponent<StackableItem>().UseRangedAmmo(playerScript.targetNPCPreviousTile, delay);
                }
                else if (WornEquipment.weapon.GetComponent<BlowPipe>() != null)
                {
                    WornEquipment.weapon.GetComponent<BlowPipe>().UseRangedAmmo(playerScript.targetNPCPreviousTile, delay);
                }
            }

            if (useSpec)
            {
                SpecBar.instance.UseSpec();
            }



            float hitChance;
            if (maxAttRoll > maxDefRoll)
            {
                hitChance = 1 - (maxDefRoll + 2) / (2 * (maxAttRoll + 1));
            }
            else
            {
                hitChance = maxAttRoll / (2 * (maxDefRoll + 1));
            }

            float dps = (maxHit / 2) * hitChance / attackCooldown / 0.6f;

            if (WornEquipment.rubyBoltsE)
            {
                float mult = 0.2f;
                if (specialEffects && effects.zaryteCrossbow)
                {
                    mult = 0.22f;
                }
                dps = (dps * (1 - rubyBoltProcChance)) + (rubyBoltProcChance * (int)Mathf.Min(100, Mathf.Floor((float)Player.targetedNPC.GetComponent<Enemy>().hitpoints * mult)) / attackCooldown / 0.6f);
            }
            else if (WornEquipment.diamondBoltsE)
            {
                float mult = 1.15f;
                if (specialEffects && effects.zaryteCrossbow)
                {
                    mult = 1.25f;
                }
                dps = (dps * (1 - diamondBoltProcChance)) + (diamondBoltProcChance * Mathf.Floor(maxHit * 1.15f) / attackCooldown / 0.6f);
            }

            CombatInfo.PlayerAttack(AttackStyles.attackStyle, (int)maxAttRoll);
            CombatInfo.PlayerAttackResult(hitChance, (int)maxHit, dps);
        }
        else
        {
            playerScript.attackThisTick = false;
        }
    }
    public void PlayerCastSpell(Spell spell)
    {
        if (onPlayer == false)
        {
            return;
        }

        playerScript.attackThisTick = false;

        if (spell.available == false)
        {
            spell.SpellNotAvailable();
            if ((AttackStyles.selectedStyle == 8 || AttackStyles.selectedStyle == 9) && spell == AttackStyles.currentSpellOnAutocast)
            {
                AttackStyles.instance.RemoveAutocast();
            }
            Player.targetedNPC = null;
            Player.attackTargetedNPC = false;

            return;
        }

        if (attackCooldown <= 0)
        {
            Prayer.UpdatePrayerBonuses();
            attackCooldown = 5;

            specialEffects = false;
            effects = null;
            if (WornEquipment.weapon != null && WornEquipment.weapon.GetComponent<SpecialEffects>() != null)
            {
                specialEffects = true;
                effects = WornEquipment.weapon.GetComponent<SpecialEffects>();
            }

            float maxAttRoll = PlayerAttackRoll();
            if (specialEffects)
            {
                maxAttRoll *= Mathf.Floor(effects.AttackRollMult());
            }


            List<Enemy> enemies = new List<Enemy>();
            enemies.Add(Player.targetedNPC.GetComponent<Enemy>());
            if (spell.aoeSize > 1)
            {
                RaycastHit2D[] cast = Physics2D.BoxCastAll(Player.targetedNPC.transform.position, Vector2.one * 5, 0, Vector2.zero);
                foreach (RaycastHit2D enemy in cast)
                {
                    if (enemy.collider.GetComponent<Enemy>() != null)
                    {
                        Enemy script = enemy.collider.GetComponent<Enemy>();
                        if ((script.npcScript.trueTile - Player.targetedNPC.trueTile).magnitude < 1.8f && enemies.Count < 9 && enemies.Contains(script) == false)
                        {
                            enemies.Add(script);
                        }
                    }
                }
            }

            playerScript.attackThisTick = true;
            int dist = TileManager.TileDistance(playerScript.trueTile, Player.targetedNPC.trueTile);
            int delay = 1 + Mathf.FloorToInt((1 + dist) / 3);
            XPDrop.SkillXPDrop("Magic", Mathf.FloorToInt((spell.levelReq / 2) + 5));
            spell.UseRunes();
            SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, spell.projectileColor, "");
            float maxHit = 0;
            foreach (Enemy enemy in enemies)
            {
                float maxDefRoll = EnemyDefenseRoll(enemy, AttackStyles.magicStyle);
                float attRoll = Random.Range(0, (int)maxAttRoll);
                float defRoll = Random.Range(0, (int)maxDefRoll);
                bool success = false;
                maxHit = Mathf.Floor((float)spell.maxDamage * (1 + ((float)WornEquipment.magicDamage / 100)));
                if (specialEffects)
                {
                    maxHit = Mathf.Floor(maxHit * effects.MaxHitMult());
                }
                if (attRoll > defRoll)
                {
                    success = true;
                }
                else if (specialEffects && (effects.rerollAttMult || effects.rerollDefMult))
                {
                    if (effects.rerollAttMult)
                    {
                        attRoll = Random.Range(0, (int)maxAttRoll);
                    }
                    if (effects.rerollDefMult)
                    {
                        defRoll = Random.Range(0, (int)maxDefRoll);
                    }

                    if (attRoll > defRoll)
                    {
                        success = true;
                    }
                }

                int hitRoll = 0;
                if (success)
                {
                    hitRoll = Random.Range(0, (int)maxHit + 1);
                }

                if (hitRoll > 0)
                {
                    XPDrop.CombatXPDrop(AttackStyles.attackStyle, AttackStyles.attackType, hitRoll, offensivePrayerOn);
                    Debug.Log(AttackStyles.attackStyle + " " + AttackStyles.attackType);
                }

                if (success)
                {
                    enemy.AddToDamageQueue(hitRoll, delay, true, (int)maxHit);
                    enemy.SpellEffects(spell);
                    if (spell.leech)
                    {
                        int heal = (int)Mathf.Floor((float)hitRoll * (float)spell.leechPercent / 100);
                        PlayerStats.PlayerHeal(heal);
                    }
                }
                else
                {
                    if (enemy.autoRetaliate)
                    {
                        enemy.AttackPlayer();
                    }
                }
            }

            if (playerScript.attackUsingSpell && playerScript.spellBeingUsed == spell)
            {
                Player.attackTargetedNPC = false;
            }
            playerScript.attackUsingSpell = false;
            playerScript.spellBeingUsed = null;

            float hitChance;
            float calcDefRoll = EnemyDefenseRoll(enemies[0], AttackStyles.magicStyle);
            if (maxAttRoll > calcDefRoll)
            {
                hitChance = 1 - (calcDefRoll + 2) / (2 * (maxAttRoll + 1));
            }
            else
            {
                hitChance = maxAttRoll / (2 * (calcDefRoll + 1));
            }

            float dps = (maxHit / 2) * hitChance / attackCooldown / 0.6f;

            CombatInfo.PlayerAttack(AttackStyles.magicStyle, (int)maxAttRoll);
            CombatInfo.PlayerAttackResult(hitChance, (int)maxHit, dps);
        }
        else
        {
            playerScript.attackThisTick = false;
        }
    }
    float PlayerAttackRoll()
    {
        float voidBonus = 1;
        float gearBonus = 1;
        float effectiveLevel;
        float styleBonus;
        float roll;
        offensivePrayerOn = false;

        if (AttackStyles.attackStyle == AttackStyles.crushStyle || AttackStyles.attackStyle == AttackStyles.slashStyle || AttackStyles.attackStyle == AttackStyles.stabStyle)
        {
            if (WornEquipment.voidMelee)
            {
                voidBonus = 1.1f;
            }

            if (Prayer.attackPrayerBonus > 1)
            {
                offensivePrayerOn = true;
            }

            if (WornEquipment.slayerHelm)
            {
                gearBonus += 0.1667f;
            }

            effectiveLevel = Mathf.Floor((Mathf.Floor(PlayerStats.currentAttack * Prayer.attackPrayerBonus) + AttackStyles.attBonus + 8) * voidBonus);
            styleBonus = WornEquipment.attackCrush;
            if (AttackStyles.attackStyle == AttackStyles.slashStyle)
            {
                styleBonus = WornEquipment.attackSlash;
            }
            else if (AttackStyles.attackStyle == AttackStyles.stabStyle)
            {
                styleBonus = WornEquipment.attackStab;
            }
        }
        else if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
        {
            if (WornEquipment.voidRange)
            {
                voidBonus = 1.1f;
            }

            if (Prayer.rangedAttackPrayerBonus > 1)
            {
                offensivePrayerOn = true;
            }

            if (WornEquipment.slayerHelm)
            {
                gearBonus += 0.15f;
            }

            if (WornEquipment.crystalWeapon)
            {
                gearBonus += WornEquipment.crystalBonus;
            }

            effectiveLevel = Mathf.Floor((Mathf.Floor(PlayerStats.currentRanged * Prayer.rangedAttackPrayerBonus) + AttackStyles.rangedBonus + 8) * voidBonus);
            styleBonus = WornEquipment.attackRange;
        }
        else //if (style == AttackStyles.magicStyle)
        {
            if (WornEquipment.voidMage)
            {
                voidBonus = 1.45f;
            }

            if (Prayer.magicAttackPrayerBonus > 1)
            {
                offensivePrayerOn = true;
            }

            if (WornEquipment.slayerHelm)
            {
                gearBonus += 0.15f;
            }

            effectiveLevel = Mathf.Floor(Mathf.Floor(PlayerStats.currentMagic * Prayer.magicAttackPrayerBonus) * voidBonus + AttackStyles.magicBonus + 9);
            styleBonus = WornEquipment.attackMagic;
        }


        roll = Mathf.Floor(effectiveLevel * (styleBonus + 64) * gearBonus);

        if (specialEffects)
        {
            roll = Mathf.Floor(roll * effects.AttackRollMult());
        }
        if (useSpec)
        {
            roll = Mathf.Floor(roll * specScript.AccuracyRollMult());
        }

        return roll;
    }
    bool SuccessfulAttack(float maxAttRoll, float maxDefRoll)
    {
        float attRoll = Random.Range(0, (int)maxAttRoll);
        float defRoll = Random.Range(0, (int)maxDefRoll);

        if (attRoll > defRoll)
        {
            return true;
        }
        if (useSpec && specScript.guaranteeHit)
        {
            return true;
        }
        if (specialEffects && (effects.rerollAttMult || effects.rerollDefMult))
        {
            if (effects.rerollAttMult)
            {
                attRoll = Random.Range(0, (int)maxAttRoll);
            }
            if (effects.rerollDefMult)
            {
                defRoll = Random.Range(0, (int)maxDefRoll);
            }

            if (attRoll > defRoll)
            {
                return true;
            }
        }

        return false;
    }
    float PlayerMaxHit()
    {
        float voidBonus = 1;
        float gearBonus = 1;
        float effectiveStrength;
        float maxHit = 0;

        if (AttackStyles.attackStyle == AttackStyles.crushStyle || AttackStyles.attackStyle == AttackStyles.slashStyle || AttackStyles.attackStyle == AttackStyles.stabStyle)
        {
            if (WornEquipment.voidMelee)
            {
                voidBonus = 1.1f;
            }

            if (Prayer.strengthPrayerBonus > 1)
            {
                offensivePrayerOn = true;
            }

            effectiveStrength = Mathf.Floor((Mathf.Floor(PlayerStats.currentStrength * Prayer.strengthPrayerBonus) + AttackStyles.strBonus + 8) * voidBonus);
            maxHit = Mathf.Floor(0.5f + (effectiveStrength * (WornEquipment.meleeStrength + 64)) / 640) * gearBonus;
        }
        else if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
        {
            if (WornEquipment.voidRange)
            {
                voidBonus = 1.1f;
                if (WornEquipment.eliteVoid)
                {
                    voidBonus = 1.125f;
                }
            }

            if (WornEquipment.slayerHelm)
            {
                gearBonus += 0.15f;
            }

            if (WornEquipment.crystalWeapon)
            {
                gearBonus += WornEquipment.crystalBonus / 2;
            }

            effectiveStrength = Mathf.Floor((Mathf.Floor(PlayerStats.currentRanged * Prayer.rangedStrengthPrayerBonus) + AttackStyles.rangedBonus + 8) * voidBonus);
            maxHit = Mathf.Floor(0.5f + (effectiveStrength * (WornEquipment.rangedStrength + 64)) / 640) * gearBonus;
        }
        else //if (style == AttackStyles.magicStyle)
        {
            if (WornEquipment.voidMage && WornEquipment.eliteVoid)
            {
                voidBonus = 1.025f;
            }

            if (WornEquipment.slayerHelm)
            {
                gearBonus += 0.15f;
            }
            gearBonus--;

            float baseMaxHit = WornEquipment.weapon.GetComponent<PoweredStaff>().BaseMaxHit(PlayerStats.currentMagic);
            maxHit = Mathf.Floor(baseMaxHit * (voidBonus + (WornEquipment.magicDamage / 100) + gearBonus));
        }

        if (specialEffects)
        {
            maxHit = Mathf.Floor(maxHit * effects.MaxHitMult());
        }
        if (useSpec)
        {
            maxHit = Mathf.Floor(maxHit * specScript.MaxHitMult());
        }

        return Mathf.Floor(maxHit);
    }
    int CalculateDelay()
    {
        int dist = TileManager.TileDistance(playerScript.trueTile, Player.targetedNPC.trueTile);
        if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
        {
            return 1 + Mathf.FloorToInt((3 + dist) / 6);
        }
        else if (AttackStyles.attackStyle == AttackStyles.magicStyle || (useSpec && specScript.slowProjectile))
        {
            return 1 + Mathf.FloorToInt((1 + dist) / 3);
        }

        return 1;
    }
    float PlayerDefenseRoll(string style)   
    {
        float gearBonus = 1;
        float effectiveLevel;
        float styleBonus;
        float roll;

        string styleText = "crush";
        if (style != AttackStyles.magicStyle)
        {
            effectiveLevel = Mathf.Floor(Mathf.Floor(PlayerStats.currentDefence * Prayer.defensePrayerBonus) + AttackStyles.defBonus + 8);
            styleBonus = WornEquipment.defenceCrush;
            if (style == AttackStyles.slashStyle)
            {
                styleText = "slash";
                styleBonus = WornEquipment.defenceSlash;
            }
            else if (style == AttackStyles.stabStyle)
            {
                styleText = "stab";
                styleBonus = WornEquipment.defenceStab;
            }
            else if (style == AttackStyles.rangedStyle)
            {
                styleText = "ranged";
                styleBonus = WornEquipment.defenceRange;
            }
        }
        else //if (style == AttackStyles.magicStyle)
        {
            styleText = "magic";
            float compositeLevel = PlayerStats.currentMagic * Prayer.magicAttackPrayerBonus * 0.7f + PlayerStats.currentDefence * 0.3f * Prayer.defensePrayerBonus;
            effectiveLevel = Mathf.Floor(Mathf.Floor(compositeLevel) + 8);
            styleBonus = WornEquipment.defenceMagic;
        }

        roll = Mathf.Floor(effectiveLevel * (styleBonus + 64) * gearBonus);

        CombatInfo.PlayerDefense((int)styleBonus, (int)roll, styleText);

        return roll;
    }

    void BeforeTick()
    {
        if (attackCooldown > minCoolDown)
        {
            attackCooldown--;
        }
    }

    public bool InAttackRange(Vector2 playerTile, Vector2 NPCSWTile, int attackRange, int NPCSize)
    {
        bool inRange = false;
        float closestX = Mathf.Clamp(playerTile.x, NPCSWTile.x, NPCSWTile.x + (NPCSize - 1));
        float closestY = Mathf.Clamp(playerTile.y, NPCSWTile.y, NPCSWTile.y + (NPCSize - 1));
        Vector2 closestTile = new Vector2(closestX, closestY);
        if (attackRange == 1)
        {
            if ((closestTile - playerTile).magnitude < 1.1f)
            {
                inRange = true;
            }
        }
        else
        {
            if (TileManager.TileDistance(playerTile, closestTile) <= attackRange)
            {
                inRange = true;
            }
        }

        return inRange;
    }
    public int DistanceToCombat(Vector2 playerTile, Vector2 NPCSWTile, int NPCSize)
    {
        float closestX = Mathf.Clamp(playerTile.x, NPCSWTile.x, NPCSWTile.x + (NPCSize - 1));
        float closestY = Mathf.Clamp(playerTile.y, NPCSWTile.y, NPCSWTile.y + (NPCSize - 1));
        Vector2 closestTile = new Vector2(closestX, closestY);
        
        return TileManager.TileDistance(playerTile, closestTile);
    }
    public bool PlayerInsideEnemy(Enemy enemyScript)
    {
        bool inside = false;

        float size = enemyScript.npcScript.tileSize;
        Vector2 enemyTile = enemyScript.npcScript.trueTile;
        Vector2 playerTile = playerScript.trueTile;

        // enemy true tile is the SOUTH WESTERN most tile
        if (playerTile.x >= enemyTile.x && playerTile.x < enemyTile.x + size && 
            playerTile.y >= enemyTile.y && playerTile.y < enemyTile.y + size)
        {
            inside = true;
        }


        return inside;
    }
    public bool AdjacentTileAvailable(Vector2 SWTile, int size)
    {
        //scans tiles left of left column and right of right column
        for (int j = 0; j < size; j++)
        {
            Vector2 newTile = SWTile + Vector2.up * j + Vector2.left;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                return true;
            }
            newTile = SWTile + Vector2.right * (size - 1) + Vector2.up * j + Vector2.right;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                return true;
            }
        }
        //scans tiles below bottom row and above top row
        for (int i = 0; i < size; i++)
        {
            Vector2 newTile = SWTile + Vector2.right * i + Vector2.down;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                return true;
            }
            newTile = SWTile + Vector2.up * (size - 1) + Vector2.right * i + Vector2.up;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                return true;
            }
        }

        return false;
    }
    public Vector2 FindEnemyAdjacentTile(Vector2 SWTile, int size)
    {
        List<Vector2> adjacentTiles = new List<Vector2>();
        //scans tiles left of left column and right of right column
        for (int j = 0; j < size; j++)
        {
            Vector2 newTile = SWTile + Vector2.up * j + Vector2.left;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
            newTile = SWTile + Vector2.right * (size - 1) + Vector2.up * j + Vector2.right;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
        }
        //scans tiles below bottom row and above top row
        for (int i = 0; i < size; i++)
        {
            Vector2 newTile = SWTile + Vector2.right * i + Vector2.down;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
            newTile = SWTile + Vector2.up * (size - 1) + Vector2.right * i + Vector2.up;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
        }

        float distance = 10000;
        Vector2 targetTile = adjacentTiles[0];
        foreach (Vector2 tile in adjacentTiles)
        {
            float newDist = TileManager.TileDistance(playerScript.trueTile, tile);
            if (newDist < distance)
            {
                targetTile = tile;
                distance = newDist;
            }
            if (newDist == distance)
            {
                if ((tile - playerScript.trueTile).magnitude < (targetTile - playerScript.trueTile).magnitude)
                {
                    targetTile = tile;
                }
            }
        }

        return targetTile;
    }
    public Vector2 FindPlayerAdjacentTile(Vector2 NPCStart, Vector2 playerTile, int size)
    {
        List<Vector2> adjacentTiles = new List<Vector2>();
        //scans tiles below bottom row and above top row
        for (int i = 0; i < size; i++)
        {
            Vector2 newTile = playerTile + Vector2.down * size + Vector2.left * i;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
            newTile = playerTile + Vector2.up + Vector2.left * i;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
        }
        //scans tiles left of left column and right of right column
        for (int j = 0; j < size; j++)
        {
            Vector2 newTile = playerTile + Vector2.left * size + Vector2.down * j;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
            newTile = playerTile + Vector2.right + Vector2.down * j;
            if (TileDataManager.GetTileData(newTile).obstacle == false)
            {
                adjacentTiles.Add(newTile);
            }
        }

        float distance = 10000;
        Vector2 targetTile = adjacentTiles[0];
        foreach (Vector2 tile in adjacentTiles)
        {
            float newDist = TileManager.TileDistance(NPCStart, tile);
            if (newDist < distance)
            {
                distance = newDist;
                targetTile = tile;
            }
        }

        if (targetTile == NPCStart + Vector2.up || targetTile == NPCStart + Vector2.down)
        {
            if (TileDataManager.GetTileData(NPCStart + Vector2.right * Mathf.Sign(playerTile.x - NPCStart.x)).obstacle)
            {
                targetTile = NPCStart;
            }
        }

        return targetTile;
    }

    public void SpawnProjectile(Vector2 target, GameObject source, int airborneTicks, Color color, Sprite projectileSprite)
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.airborneTicks = airborneTicks;
        script.targetPosition = target;
        script.color = color;
        script.source = source;
        script.customSprite = projectileSprite;
        script.appearInstantly = projectileSpawnsImmediately;
        script.transform.localScale *= projectileSize;
    }
    public void SpawnProjectile(GameObject target, GameObject source, int airborneTicks, Color color, Sprite projectileSprite)
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.airborneTicks = airborneTicks;
        script.target = target;
        script.color = color;
        script.source = source;
        script.customSprite = projectileSprite;
        script.appearInstantly = projectileSpawnsImmediately;
        script.transform.localScale *= projectileSize;
    }
    public void SpawnProjectile(GameObject target, GameObject source, int airborneTicks, Color color, string WeaponCategory)
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.airborneTicks = airborneTicks;
        script.target = target;
        script.color = color;
        script.source = source;
        script.appearInstantly = projectileSpawnsImmediately;
        if (onPlayer == false && source.GetComponent<Enemy>().customProjectile != null)
        {
            script.customSprite = source.GetComponent<Enemy>().customProjectile;
        }
        else
        {
            if (WeaponCategory == WornEquipment.bowCategory)
            {
                script.arrow = true;
                newProjectile.transform.localScale *= 0.8f;
            }
            if (WeaponCategory == WornEquipment.crossbowCategory)
            {
                script.bolt = true;
            }
            if (WeaponCategory == WornEquipment.thrownCategory)
            {
                script.dart = true;
            }
        }
        script.transform.localScale *= projectileSize;
    }


    public void EnemyAttack(Enemy enemyScript)
    {
        if (onPlayer)
        {
            return;
        }

        enemyScript.attackThisTick = false;

        Prayer.UpdatePrayerBonuses();

        if (attackCooldown <= 0)
        {
            style = enemyScript.attackStyle;
            if (enemyScript.useMeleeUpClose && DistanceToCombat(playerScript.trueTile, enemyScript.npcScript.trueTile, enemyScript.npcScript.tileSize) == 1)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < enemyScript.percentMeleeUpClose)
                {
                    style = enemyScript.meleeStyle;
                }
            }

            attackCooldown = enemyScript.attackSpeed;

            float maxAttRoll = EnemyAttackRoll(enemyScript);
            float maxDefRoll = PlayerDefenseRoll(style);
            float attRoll = Random.Range(0, (int)maxAttRoll);
            float defRoll = Random.Range(0, (int)maxDefRoll);
            bool success = false;
            if (attRoll > defRoll || enemyScript.ignoreAccuracyCheck)
            {
                success = true;
            }

            float maxHit = EnemyMaxHit(enemyScript);
            int hitRoll = 0;
            if (success)
            {
                hitRoll = Random.Range(0, (int)maxHit + 1);
            }

            if ((Prayer.protectFromMelee && melee) || (Prayer.protectFromMissiles && range) || (Prayer.protectFromMagic && mage))
            {
                if (enemyScript.typelessAttack == false)
                {
                    hitRoll = Mathf.FloorToInt((float) hitRoll * (1 - enemyScript.overheadProtectionMult));
                }
            }

            int dist = TileManager.TileDistance(playerScript.trueTile, enemyScript.npcScript.trueTile);
            int delay = 1;
            string type = "";
            if (range && enemyScript.slowProjectile == false)
            {
                delay = 1 + Mathf.FloorToInt((3 + dist) / 6);
                type = WornEquipment.bowCategory;
            }
            else if ((mage || enemyScript.slowProjectile) && melee == false)
            {
                delay = 1 + Mathf.FloorToInt((1 + dist) / 3);
            }
            if (enemyScript.uniformProjectileDelay > 0 && melee == false)
            {
                delay = enemyScript.uniformProjectileDelay;
            }
            if (melee == false)
            {
                SpawnProjectile(playerScript.gameObject, enemyScript.gameObject, delay, enemyScript.projectileColor, type);
            }

            playerScript.AddToDamageQueue(hitRoll, delay, enemyScript);
            enemyScript.attackThisTick = true;

            float hitChance;
            if (maxAttRoll > maxDefRoll)
            {
                hitChance = 1 - (maxDefRoll + 2) / (2 * (maxAttRoll + 1));
            }
            else
            {
                hitChance = maxAttRoll / (2 * (maxDefRoll + 1));
            }

            float dps = (maxHit / 2) * hitChance / attackCooldown / 0.6f;

            CombatInfo.EnemyAttackResult(hitChance, (int) maxHit, dps);
        }
        else
        {
            enemyScript.attackThisTick = false;
        }
    }
    float EnemyAttackRoll(Enemy enemyScript)
    {
        float effectiveLevel;
        float styleBonus;
        float roll;

        melee = true;
        mage = false;
        range = false;
        int level = enemyScript.attack;
        effectiveLevel = Mathf.Floor(Mathf.Floor(enemyScript.attack) + 8);
        styleBonus = enemyScript.attackCrush;
        if (style == AttackStyles.slashStyle)
        {
            styleBonus = enemyScript.attackSlash;
        }
        else if (style == AttackStyles.stabStyle)
        {
            styleBonus = enemyScript.attackStab;
        }
        else if (style == AttackStyles.rangedStyle)
        {
            range = true;
            melee = false;
            level = enemyScript.ranged;
            effectiveLevel = Mathf.Floor(Mathf.Floor(enemyScript.ranged) + 8);
            styleBonus = enemyScript.attackRange;
        }
        else if (style == AttackStyles.magicStyle)
        {
            mage = true;
            melee = false;
            level = enemyScript.magic;
            effectiveLevel = Mathf.Floor(Mathf.Floor(enemyScript.magic) + 9);
            styleBonus = enemyScript.attackMagic;
        }

        roll = Mathf.Floor(effectiveLevel * (styleBonus + 64));

        CombatInfo.EnemyAttack(enemyScript.name, style, level, (int)styleBonus, (int)roll);

        return roll;
    }
    public float EnemyMaxHit(Enemy enemyScript)
    {
        float effectiveStrength;
        float maxHit;

        if (enemyScript.willOverrideMaxHit)
        {
            return enemyScript.newMaxHit;
        }

        if (style != AttackStyles.magicStyle)
        {
            float level = enemyScript.strength;
            float strength = enemyScript.meleeStrength;
            if (style == AttackStyles.rangedStyle)
            {
                level = enemyScript.ranged;
                strength = enemyScript.rangedStrength;
            }

            effectiveStrength = Mathf.Floor(Mathf.Floor(level) + 8);
            maxHit = Mathf.Floor(0.5f + effectiveStrength * (strength + 64) / 640);
        }
        else
        {
            effectiveStrength = Mathf.Floor(Mathf.Floor(enemyScript.magic) + 8);
            maxHit = Mathf.Floor(0.5f + effectiveStrength / 5 * (1 + enemyScript.magicDamage/100));
        }

        return Mathf.Floor(maxHit);
    }
    float EnemyDefenseRoll(Enemy enemyScript, string style)
    {
        float defenseBonus = enemyScript.defenceCrush;
        if (style == AttackStyles.slashStyle)
        {
            defenseBonus = enemyScript.defenceSlash;
        }
        else if (style == AttackStyles.stabStyle)
        {
            defenseBonus = enemyScript.defenceStab;
        }
        else if (style == AttackStyles.rangedStyle)
        {
            defenseBonus = enemyScript.defenceRange;
        }
        else if (style == AttackStyles.magicStyle)
        {
            defenseBonus = enemyScript.defenceMagic;
        }

        float roll = (enemyScript.defence + 9) * (defenseBonus + 64);
        bool magic = false;
        if (style == AttackStyles.magicStyle)
        {
            roll = (enemyScript.magic + 9) * (defenseBonus + 64);
            magic = true;
        }

        CombatInfo.EnemyDefense(enemyScript.defence, (int)defenseBonus, (int)roll, magic, style.ToLower());

        return roll;
    }

    public bool LineOfSight(NPC npcScript)
    {
        float NPCClosestTileX = Mathf.Clamp(Player.player.trueTile.x, npcScript.trueTile.x, npcScript.trueTile.x + npcScript.tileSize - 1);
        float NPCClosestTiley = Mathf.Clamp(Player.player.trueTile.y, npcScript.trueTile.y, npcScript.trueTile.y + npcScript.tileSize - 1);
        Vector2 NPCtile = new Vector2(NPCClosestTileX, NPCClosestTiley);
        Vector2 relativeTile = NPCtile - Player.player.trueTile;

        if (relativeTile == Vector2.zero)
        {
            Debug.Log("line of sight starts and ends on the same tile");
            return false;
        }

        Vector2 lineStart;
        //prioritize up / down borders when it's perfectly diagonal
        if (Mathf.Abs(relativeTile.x) > Mathf.Abs(relativeTile.y))
        {
            lineStart = new Vector2(Mathf.Sign(relativeTile.x) * 0.5f, 0);
        }
        else
        {
            lineStart = new Vector2(0, Mathf.Sign(relativeTile.y) * 0.5f);
        }

        float slope = relativeTile.y / relativeTile.x;
        float b = lineStart.y;
        if (relativeTile.x != 0 && lineStart.x != 0)
        {
            b = -lineStart.x * slope;
        }

        if (debug)
        {
            line.SetPosition(0, Player.player.trueTile + lineStart);
            line.SetPosition(1, Player.player.trueTile + relativeTile + lineStart);
            line.startColor = Color.red;
            line.endColor = Color.red;
            foreach(GameObject intersection in intersections)
            {
                Destroy(intersection);
            }
            intersections = new List<GameObject>();
            foreach (GameObject tile in checkedTileMarkers)
            {
                Destroy(tile);
            }
            checkedTileMarkers = new List<GameObject>();
        }

        Vector2 CoordinateFromX(float x)
        {
            //make sure not to do this when slope = infinity;
            Vector2 coordinate = Player.player.trueTile + new Vector2(x, slope * x + b);
            return coordinate;
        }
        Vector2 CoordinateFromY(float y)
        {
            //make sure not to do this when slope = 0;
            Vector2 coordinate = Player.player.trueTile + new Vector2((y - b) / slope, y);
            return coordinate;
        }

        List<Vector2> verticalIntersections = new List<Vector2>();
        List<Vector2> horizontalIntersections = new List<Vector2>();
        List<Vector2> cornerIntersections = new List<Vector2>();

        //gather list of vertical intersections and corner intersections. ignore section if slope is infinity.
        float i = 0;
        while (i != relativeTile.x && relativeTile.x != 0)
        {
            float x = i + Mathf.Sign(relativeTile.x) * 0.5f;
            Vector2 coordinate = CoordinateFromX(x);
            if (Mathf.Abs(coordinate.y - Mathf.Floor(coordinate.y) - 0.5f) < 0.01f)
            {
                if (cornerIntersections.Contains(coordinate) == false)
                {
                    cornerIntersections.Add(coordinate);
                }
            }
            else
            {
                verticalIntersections.Add(coordinate);
            }
            i += Mathf.Sign(relativeTile.x);
        }
        //gather list of horizontal intersections and corner intersections. ignore section if slope is 0.
        float j = 0;
        while (j != relativeTile.y && relativeTile.y != 0)
        {
            float y = j + Mathf.Sign(relativeTile.y) * 0.5f;
            Vector2 coordinate = CoordinateFromY(y);
            if (Mathf.Abs(coordinate.x - Mathf.Floor(coordinate.x) - 0.5f) < 0.01f)
            {
                if (cornerIntersections.Contains(coordinate) == false)
                {
                    cornerIntersections.Add(coordinate);
                }
            }
            else
            {
                horizontalIntersections.Add(coordinate);
            }
            j += Mathf.Sign(relativeTile.y);
        }

        List<Vector2> searchedTiles = new List<Vector2>();
        //go through each vertical intersection, get tile data from left and right tiles. If either are obstacles, return false. Ignore tiles already examined.
        foreach (Vector2 intersection in verticalIntersections)
        {
            if (debug)
            {
                GameObject newIntersection = Instantiate(LOSIntersection, intersection, Quaternion.identity);
                intersections.Add(newIntersection);
            }

            for (i = -1; i < 2; i += 2)
            {
                Vector2 tile = TileManager.FindTile(intersection + Vector2.right * i * 0.5f);
                if (searchedTiles.Contains(tile) == false)
                {
                    if (debug)
                    {
                        GameObject newTile = Instantiate(checkedTileMarker, tile, Quaternion.identity);
                        checkedTileMarkers.Add(newTile);
                    }

                    if (TileDataManager.GetTileData(tile).tallObstacle)
                    {
                        if (debug)
                        {
                            checkedTileMarkers[checkedTileMarkers.Count - 1].GetComponent<SpriteRenderer>().color = Color.red;
                            intersections[intersections.Count - 1].GetComponent<SpriteRenderer>().color = Color.red;
                        }

                        return false;
                    }
                    searchedTiles.Add(tile);
                }
            }
        }

        //same for horizontal intersections, up and down tiles.
        foreach (Vector2 intersection in horizontalIntersections)
        {
            if (debug)
            {
                GameObject newIntersection = Instantiate(LOSIntersection, intersection, Quaternion.identity);
                intersections.Add(newIntersection);
            }

            for (i = -1; i < 2; i += 2)
            {
                Vector2 tile = TileManager.FindTile(intersection + Vector2.up * i * 0.5f);
                if (searchedTiles.Contains(tile) == false)
                {
                    if (debug)
                    {
                        GameObject newTile = Instantiate(checkedTileMarker, tile, Quaternion.identity);
                        checkedTileMarkers.Add(newTile);
                    }

                    if (TileDataManager.GetTileData(tile).tallObstacle)
                    {
                        if (debug)
                        {
                            checkedTileMarkers[checkedTileMarkers.Count - 1].GetComponent<SpriteRenderer>().color = Color.red;
                            intersections[intersections.Count - 1].GetComponent<SpriteRenderer>().color = Color.red;
                        }

                        return false;
                    }
                    searchedTiles.Add(tile);
                }
            }
        }

        foreach (Vector2 intersection in cornerIntersections)
        {
            if (debug)
            {
                GameObject newIntersection = Instantiate(LOSIntersection, intersection, Quaternion.identity);
                intersections.Add(newIntersection);
            }

            for (i = 45; i < 360; i += 90)
            {
                Vector2 tile = TileManager.FindTile(intersection + Tools.AngleToVector(i));
                if (searchedTiles.Contains(tile) == false)
                {
                    if (debug)
                    {
                        GameObject newTile = Instantiate(checkedTileMarker, tile, Quaternion.identity);
                        checkedTileMarkers.Add(newTile);
                    }

                    if (TileDataManager.GetTileData(tile).tallObstacle)
                    {
                        if (debug)
                        {
                            checkedTileMarkers[checkedTileMarkers.Count - 1].GetComponent<SpriteRenderer>().color = Color.yellow;
                            intersections[intersections.Count - 1].GetComponent<SpriteRenderer>().color = Color.yellow;
                        }

                        Debug.Log("there are corners that need to be accounted for");
                    }
                }
            }
        }

        if (debug)
        {
            line.startColor = Color.green;
            line.endColor = Color.green;
        }

        return true;
    }

    public bool CapableOfAttacking(Vector2 playerTile, Vector2 NPCPreviousTile, NPC NPCScript, int attackRange, bool ignoreLOS)
    {
        if (InAttackRange(playerTile, NPCPreviousTile, attackRange, NPCScript.tileSize) == false)
        {
            //Debug.Log("failed InAttackRange");
            return false;
        }
        if (PlayerInsideEnemy(NPCScript.GetComponent<Enemy>()))
        {
            //Debug.Log("failed PlayerInsideEnemy");
            return false;
        }
        if (ignoreLOS == false && LineOfSight(NPCScript) == false)
        {
            //Debug.Log("failed LineOfSight");
            return false;
        }

        return true;
    }
}
