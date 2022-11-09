using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    Player playerScript;
    Pathfinder pathFinder;
    [HideInInspector] public Enemy targetEnemy;
    bool specialEffects;
    SpecialEffects effects;

    int attackCooldown = 0;

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
        Prayer.UpdatePrayerBonuses();

        if (attackCooldown <= 0)
        {
            playerScript.attackThisTick = true;
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
                maxAttRoll *= Mathf.Floor(effects.attackRollMult());
            }



            float maxDefRoll = EnemyDefenseRoll(Player.targetedNPC.GetComponent<Enemy>());




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
                    maxHit = Mathf.Floor(maxHit * effects.maxHitMult());
                }

                hitRoll = Random.Range(0, (int)maxHit + 1);
            }

            Player.targetedNPC.GetComponent<Enemy>().DealDamageToEnemy(hitRoll, 1);


            float hitChance;
            if (maxAttRoll > maxDefRoll)
            {
                hitChance = 1 - (maxDefRoll + 2) / (2 * (maxAttRoll + 1));
            }
            else
            {
                hitChance = maxAttRoll / (2 * (maxDefRoll + 1));
            }

            Debug.Log(AttackStyles.attackStyle + ", " + AttackStyles.attackType + ". attack roll: " + maxAttRoll + ". def roll: " + maxDefRoll + ". hitchance: " + hitChance * 100 + ". max hit: " + maxHit + ". success: " + success);

        }
        else
        {
            playerScript.attackThisTick = false;
        }
    }

    void BeforeTick()
    {
        if (attackCooldown > -6)
        {
            attackCooldown--;
        }
    }

    public bool InAttackRange(Vector2 start, Vector2 end, int attackRange)
    {
        bool inRange = false;

        if (attackRange == 1)
        {
            if ((end - start).magnitude > 0.9f && (end - start).magnitude < 1.1f)
            {
                inRange = true;
            }
        }
        else
        {
            if (TileManager.TileDistance(start, end) <= attackRange && (end - start).magnitude > 0.9f)
            {
                inRange = true;
            }
        }

        return inRange;
    }
    public bool AdjacentTileAvailable(Vector2 end)
    {
        if (TileDataManager.GetTileData(end + Vector2.left).obstacle == false)
        {
            return true;
        }
        else if (TileDataManager.GetTileData(end + Vector2.right).obstacle == false)
        {
            return true;
        }
        else if (TileDataManager.GetTileData(end + Vector2.up).obstacle == false)
        {
            return true;
        }
        else if (TileDataManager.GetTileData(end + Vector2.down).obstacle == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector2 FindAdjacentTile(Vector2 end)
    {
        List<Vector2> adjacentTiles = new List<Vector2>();
        if (TileDataManager.GetTileData(end + Vector2.left).obstacle == false)
        {
            adjacentTiles.Add(end + Vector2.left);
        }
        if (TileDataManager.GetTileData(end + Vector2.right).obstacle == false)
        {
            adjacentTiles.Add(end + Vector2.right);
        }
        if (TileDataManager.GetTileData(end + Vector2.up).obstacle == false)
        {
            adjacentTiles.Add(end + Vector2.up);
        }
        if (TileDataManager.GetTileData(end + Vector2.down).obstacle == false)
        {
            adjacentTiles.Add(end + Vector2.down);
        }

        float distance = 10000;
        Vector2 targetTile = adjacentTiles[0];
        foreach (Vector2 tile in adjacentTiles)
        {
            List<Vector2> path = pathFinder.FindAStarPath(playerScript.trueTile, tile);
            if (path.Count < distance)
            {
                targetTile = tile;
                distance = path.Count;
            }
        }

        return targetTile;
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
        }

        return Mathf.Floor(maxHit);
    }
    float EnemyDefenseRoll(Enemy enemyScript)
    {
        float defenseBonus = enemyScript.defenceCrush;
        if (AttackStyles.attackStyle == AttackStyles.slashStyle)
        {
            defenseBonus = enemyScript.defenceSlash;
        }
        else if (AttackStyles.attackStyle == AttackStyles.stabStyle)
        {
            defenseBonus = enemyScript.defenceStab;
        }
        else if (AttackStyles.attackStyle == AttackStyles.rangedStyle)
        {
            defenseBonus = enemyScript.defenceRange;
        }
        else if (AttackStyles.attackStyle == AttackStyles.magicStyle)
        {
            defenseBonus = enemyScript.defenceMagic;
        }

        float roll = (enemyScript.defence + 9) * (defenseBonus + 64);
        if (AttackStyles.attackStyle == AttackStyles.magicStyle)
        {
            roll = (enemyScript.magic + 9) * (enemyScript.defenceMagic + 64);
        }

        return roll;
    }
}
