using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hitpoints = 10;
    int initialHitpoints;

    public int attack = 1;
    public int strength = 1;
    public int defence = 1;
    int initialDefence;
    public int ranged = 1;
    public int magic = 1;

    public int attackStab;
    public int attackSlash;
    public int attackCrush;
    public int attackMagic;
    public int attackRange;

    public int defenceStab;
    public int defenceSlash;
    public int defenceCrush;
    public int defenceMagic;
    public int defenceRange;

    public int meleeStrength;
    public int rangedStrength;
    public int magicDamage;

    public int attackSpeed;

    [HideInInspector] public float combatLevel;
    [HideInInspector] public string combatLevelColor;

    NPC npcScript;
    Action npcAction;
    Player playerScript;
    TrueTile playerTrueTileScript;
    Pathfinder pathFinder;

    bool playerAttackingEnemy = false;

    private IEnumerator Start()
    {
        initialHitpoints = hitpoints;
        initialDefence = defence;

        npcScript = GetComponent<NPC>();
        npcAction = GetComponent<Action>();
        playerScript = FindObjectOfType<Player>();
        playerTrueTileScript = FindObjectOfType<TrueTile>();
        pathFinder = FindObjectOfType<Pathfinder>();

        float baselvl = 0.25f * ((float)defence + hitpoints + (1 * 0.5f));
        float meleelvl = (13f / 40f) * ((float)attack + strength);
        float rangelvl = (13f / 40f) * ((float)ranged * 3 / 2);
        float magelvl = (13f / 40f) * ((float)magic * 3 / 2);
        combatLevel = baselvl + Mathf.Max(meleelvl, rangelvl, magelvl);

        if (combatLevel >= PlayerStats.combatLevel + 10)
        {
            combatLevelColor = "#ff0000";
        }
        else if (combatLevel >= PlayerStats.combatLevel + 6)
        {
            combatLevelColor = "#ff3000";
        }
        else if (combatLevel >= PlayerStats.combatLevel + 3)
        {
            combatLevelColor = "#ff7000";
        }
        else if (Mathf.FloorToInt(combatLevel) > Mathf.FloorToInt(PlayerStats.combatLevel))
        {
            combatLevelColor = "#ffb000";
        }
        else if (Mathf.FloorToInt(combatLevel) == Mathf.FloorToInt(PlayerStats.combatLevel))
        {
            combatLevelColor = "#ffff00";
        }
        else if (combatLevel >= PlayerStats.combatLevel - 3)
        {
            combatLevelColor = "#c0ff00";
        }
        else if (combatLevel >= PlayerStats.combatLevel - 6)
        {
            combatLevelColor = "#80ff00";
        }
        else if (combatLevel >= PlayerStats.combatLevel - 9)
        {
            combatLevelColor = "#40ff00";
        }
        else
        {
            combatLevelColor = "#00ff00";
        }

        yield return null;

        npcScript.menuTexts[0] = "Attack ";
        npcAction.orderLevels[0] = -1;
        npcAction.cancelLevels[0] = 1;
        npcAction.menuPriorities[0] = 1;
        npcAction.serverAction0 += Attack;
        npcScript.UpdateActions(gameObject.name, true);

        Action.cancel1 += CancelAttack;

        TickManager.beforeTick += AttackTick;
    }

    void Attack()
    {
        playerAttackingEnemy = true;
        Player.targetedNPC = gameObject;
    }

    void AttackTick()
    {
        if (playerAttackingEnemy)
        {
            List<Vector2> adjacentTiles = new List<Vector2>();
            if (TileDataManager.GetTileData(npcScript.trueTile + Vector2.up).obstacle == false)
            {
                adjacentTiles.Add(npcScript.trueTile + Vector2.up);
            }
            if (TileDataManager.GetTileData(npcScript.trueTile + Vector2.down).obstacle == false)
            {
                adjacentTiles.Add(npcScript.trueTile + Vector2.down);
            }
            if (TileDataManager.GetTileData(npcScript.trueTile + Vector2.left).obstacle == false)
            {
                adjacentTiles.Add(npcScript.trueTile + Vector2.left);
            }
            if (TileDataManager.GetTileData(npcScript.trueTile + Vector2.right).obstacle == false)
            {
                adjacentTiles.Add(npcScript.trueTile + Vector2.right);
            }

            foreach (Vector2 tile in adjacentTiles)
            {
                if (playerTrueTileScript.currentTile == tile)
                {
                    return;
                }
            }

            int distance = 10000;
            Vector2 targetTile = adjacentTiles[0];
            foreach (Vector2 tile in adjacentTiles)
            {
                List<Vector2> path = pathFinder.FindAStarPath(playerTrueTileScript.currentTile, tile);
                if (path.Count < distance)
                {
                    targetTile = tile;
                    distance = path.Count;
                }
            }

            playerTrueTileScript.ExternalMovement(targetTile);

        }
    }

    void CancelAttack()
    {
        playerAttackingEnemy = false;
        if (Player.targetedNPC == gameObject)
        {
            Player.targetedNPC = null;
        }
    }
}
