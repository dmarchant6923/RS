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

    bool melee;
    bool mage;
    bool range;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        pathFinder = FindObjectOfType<Pathfinder>();

        TickManager.beforeTick += BeforeTick;

        specialEffects = false;
        effects = null;
    }
    public void PlayerAttack()
    {
        if (onPlayer == false)
        {
            return;
        }

        playerScript.attackThisTick = false;

        if (AttackStyles.attackStyle == AttackStyles.rangedStyle && WornEquipment.weapon.weaponCategory != WornEquipment.thrownCategory)
        {
            if (WornEquipment.ammo == null)
            {
                Debug.Log("There is no ammo left in your quiver.");
                playerScript.RemoveFocus();
                return;
            }
            if (WornEquipment.weapon.weaponCategory == WornEquipment.bowCategory && WornEquipment.ammo.ammoCategory != "Arrow")
            {
                Debug.Log("You can't use that ammo with your bow.");
                playerScript.RemoveFocus();
                return;
            }
            if (WornEquipment.weapon.weaponCategory == WornEquipment.crossbowCategory && WornEquipment.ammo.ammoCategory != "Bolt")
            {
                Debug.Log("You can't use that ammo with your crossbow.");
                playerScript.RemoveFocus();
                return;
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

        if (attackCooldown <= 0)
        {
            attackCooldown = WornEquipment.attackSpeed;
            if (AttackStyles.attackStyle == AttackStyles.rangedStyle && AttackStyles.attackType == AttackStyles.rapidType)
            {
                attackCooldown--;
            }


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
            float maxDefRoll = EnemyDefenseRoll(Player.targetedNPC.GetComponent<Enemy>(), AttackStyles.attackStyle);
            float attRoll = Random.Range(0, (int) maxAttRoll);
            float defRoll = Random.Range(0, (int) maxDefRoll);
            bool success = false;
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



            float maxHit = PlayerMaxHit();
            int hitRoll = 0;
            if (success)
            {
                if (specialEffects)
                {
                    maxHit = Mathf.Floor(maxHit * effects.MaxHitMult());
                }

                hitRoll = Random.Range(0, (int)maxHit + 1);
            }

            if (hitRoll > 0)
            {
                XPDrop.CombatXPDrop(AttackStyles.attackStyle, AttackStyles.attackType, hitRoll);
            }


            int dist = TileManager.TileDistance(playerScript.trueTile, Player.targetedNPC.trueTile);
            int delay = 1;
            if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
            {
                delay = 1 + Mathf.FloorToInt((3 + dist) / 6);
            }
            else if (AttackStyles.attackStyle == AttackStyles.magicStyle)
            {
                delay = 1 + Mathf.FloorToInt((1 + dist) / 3);
            }

            if (AttackStyles.attackStyle == AttackStyles.magicStyle && success == false)
            {

            }
            else
            {
                Player.targetedNPC.GetComponent<Enemy>().AddToDamageQueue(hitRoll, delay, true, (int)maxHit);
            }
            playerScript.attackThisTick = true;


            if (AttackStyles.attackStyle == AttackStyles.rangedStyle && WornEquipment.weapon.weaponCategory != WornEquipment.thrownCategory)
            {
                WornEquipment.ammo.GetComponent<StackableItem>().UseRangedAmmo(playerScript.targetNPCPreviousTile, delay);
            }

            if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
            {
                SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, WornEquipment.ammo.GetComponent<StackableItem>().projectileColor, WornEquipment.weapon.weaponCategory);
            }
            if (AttackStyles.attackStyle == AttackStyles.magicStyle)
            {
                SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, WornEquipment.weapon.GetComponent<PoweredStaff>().projectileColor, WornEquipment.weapon.weaponCategory);
                WornEquipment.weapon.GetComponent<ChargeItem>().UseCharge();
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
            float maxDefRoll = EnemyDefenseRoll(Player.targetedNPC.GetComponent<Enemy>(), AttackStyles.magicStyle);
            float attRoll = Random.Range(0, (int)maxAttRoll);
            float defRoll = Random.Range(0, (int)maxDefRoll);
            bool success = false;
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



            float maxHit = Mathf.Floor((float)spell.maxDamage *(1 + ((float)WornEquipment.magicDamage / 100)));
            int hitRoll = 0;
            if (success)
            {
                if (specialEffects)
                {
                    maxHit = Mathf.Floor(maxHit * effects.MaxHitMult());
                }

                hitRoll = Random.Range(0, (int)maxHit + 1);
            }

            XPDrop.SkillXPDrop("Magic", Mathf.FloorToInt((spell.levelReq / 2) + 5));
            if (hitRoll > 0)
            {
                XPDrop.CombatXPDrop(AttackStyles.attackStyle, AttackStyles.attackType, hitRoll);
            }



            int dist = TileManager.TileDistance(playerScript.trueTile, Player.targetedNPC.trueTile);
            int delay = 1 + Mathf.FloorToInt((1 + dist) / 3);
            if (success)
            {
                Player.targetedNPC.GetComponent<Enemy>().AddToDamageQueue(hitRoll, delay, true, (int)maxHit);
                Player.targetedNPC.GetComponent<Enemy>().SpellEffects(spell);
                if (spell.leech)
                {
                    int heal = (int)Mathf.Floor((float)hitRoll * (float)spell.leechPercent / 100);
                    PlayerStats.PlayerHeal(heal);
                }
            }
            playerScript.attackThisTick = true;
            spell.UseRunes();
            SpawnProjectile(Player.targetedNPC.gameObject, playerScript.gameObject, delay, spell.projectileColor, "");

            if (playerScript.attackUsingSpell && playerScript.spellBeingUsed == spell)
            {
                Player.attackTargetedNPC = false;
            }
            playerScript.attackUsingSpell = false;
            playerScript.spellBeingUsed = null;

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

        if (AttackStyles.attackStyle == AttackStyles.crushStyle || AttackStyles.attackStyle == AttackStyles.slashStyle || AttackStyles.attackStyle == AttackStyles.stabStyle)
        {
            if (WornEquipment.voidMelee)
            {
                voidBonus = 1.1f;
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

            effectiveLevel = Mathf.Floor((Mathf.Floor(PlayerStats.currentRanged * Prayer.rangedAttackPrayerBonus) + AttackStyles.rangedBonus + 8) * voidBonus);
            styleBonus = WornEquipment.attackRange;
        }
        else //if (style == AttackStyles.magicStyle)
        {
            if (WornEquipment.voidMage)
            {
                voidBonus = 1.45f;
            }

            effectiveLevel = Mathf.Floor(Mathf.Floor(PlayerStats.currentMagic * Prayer.magicAttackPrayerBonus) * voidBonus + AttackStyles.magicBonus + 9);
            styleBonus = WornEquipment.attackMagic;
        }

        roll = Mathf.Floor(effectiveLevel * (styleBonus + 64) * gearBonus);

        return roll;
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

            effectiveStrength = Mathf.Floor((Mathf.Floor(PlayerStats.currentRanged * Prayer.rangedStrengthPrayerBonus) + AttackStyles.rangedBonus + 8) * voidBonus);
            maxHit = Mathf.Floor(0.5f + (effectiveStrength * (WornEquipment.rangedStrength + 64)) / 640) * gearBonus;
        }
        else //if (style == AttackStyles.magicStyle)
        {
            if (WornEquipment.voidMage && WornEquipment.eliteVoid)
            {
                voidBonus = 1.025f;
            }
            float baseMaxHit = WornEquipment.weapon.GetComponent<PoweredStaff>().BaseMaxHit(PlayerStats.currentMagic);
            maxHit = Mathf.Floor(baseMaxHit * (voidBonus + (WornEquipment.magicDamage / 100)));
        }

        return Mathf.Floor(maxHit);
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

        return targetTile;
    }

    public void SpawnProjectile(GameObject target, GameObject source, int airborneTicks, Color color, string WeaponCategory)
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.airborneTicks = airborneTicks;
        script.target = target;
        script.color = color;
        script.source = source;
        if (WeaponCategory == WornEquipment.bowCategory)
        {
            script.arrow = true;
            newProjectile.transform.localScale *= 0.8f;
        }
        if (WeaponCategory == WornEquipment.thrownCategory)
        {
            script.dart = true;
        }
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
            attackCooldown = enemyScript.attackSpeed;

            float maxAttRoll = EnemyAttackRoll(enemyScript);
            float maxDefRoll = PlayerDefenseRoll(enemyScript.attackStyle);
            float attRoll = Random.Range(0, (int)maxAttRoll);
            float defRoll = Random.Range(0, (int)maxDefRoll);
            bool success = false;
            if (attRoll > defRoll)
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
                hitRoll = 0;
            }

            playerScript.AddToDamageQueue(hitRoll, 1, enemyScript);
            //Player.targetedNPC.GetComponent<Enemy>().DealDamageToEnemy(hitRoll, 1, true, (int)maxHit);
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
        if (enemyScript.attackStyle == AttackStyles.slashStyle)
        {
            styleBonus = enemyScript.attackSlash;
        }
        else if (enemyScript.attackStyle == AttackStyles.stabStyle)
        {
            styleBonus = enemyScript.attackStab;
        }
        else if (enemyScript.attackStyle == AttackStyles.rangedStyle)
        {
            range = true;
            level = enemyScript.ranged;
            effectiveLevel = Mathf.Floor(Mathf.Floor(enemyScript.ranged) + 8);
            styleBonus = enemyScript.attackRange;
        }
        else if (enemyScript.attackStyle == AttackStyles.magicStyle)
        {
            mage = true;
            level = enemyScript.magic;
            effectiveLevel = Mathf.Floor(Mathf.Floor(enemyScript.magic) + 9);
            styleBonus = enemyScript.attackMagic;
        }

        roll = Mathf.Floor(effectiveLevel * (styleBonus + 64));

        CombatInfo.EnemyAttack(enemyScript.attackStyle, level, (int)styleBonus, (int)roll);

        return roll;
    }
    float EnemyMaxHit(Enemy enemyScript)
    {
        float effectiveStrength;
        float maxHit;

        if (enemyScript.willOverrideMaxHit)
        {
            return enemyScript.newMaxHit;
        }

        if (enemyScript.attackStyle != AttackStyles.magicStyle)
        {
            float level = enemyScript.strength;
            float strength = enemyScript.meleeStrength;
            if (enemyScript.attackStyle == AttackStyles.rangedStyle)
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
            maxHit = Mathf.Floor(Mathf.Floor(0.5f + effectiveStrength / 640) * (1 + enemyScript.magicDamage));
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
}
