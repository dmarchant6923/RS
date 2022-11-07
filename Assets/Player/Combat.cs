using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [HideInInspector] public bool scriptAttachedToPlayer = false;
    Player playerScript;
    Pathfinder pathFinder;
    [HideInInspector] public Enemy targetEnemy;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        pathFinder = FindObjectOfType<Pathfinder>();

        if (scriptAttachedToPlayer)
        {
            TrueTile.afterMovement += PlayerAttack;
        }
    }
    public void PlayerAttack()
    {
        if (Player.targetedNPC == null || Player.attackTargetedNPC == false)
        {
            return;
        }

        Vector2 targetTile = playerScript.targetNPCPreviousTile;
        int distance = TileManager.TileDistance(playerScript.truePlayerTile, targetTile);
        bool attackSuccessful = false;
        if (WornEquipment.attackDistance == 1 && (playerScript.truePlayerTile - targetTile).magnitude < 1.1f)
        {
            attackSuccessful = true;
        }
        else if (WornEquipment.attackDistance > 1)
        {
            if (distance <= WornEquipment.attackDistance)
            {
                attackSuccessful = true;
            }
        }

        playerScript.attackThisTick = false;
        if (attackSuccessful)
        {
            playerScript.trueTile.StopMovement();
            playerScript.attackThisTick = true;
            return;
        }


        List<Vector2> adjacentTiles = new List<Vector2>();
        if (TileDataManager.GetTileData(targetEnemy.npcScript.trueTile + Vector2.left).obstacle == false)
        {
            adjacentTiles.Add(targetEnemy.npcScript.trueTile + Vector2.left);
        }
        if (TileDataManager.GetTileData(targetEnemy.npcScript.trueTile + Vector2.right).obstacle == false)
        {
            adjacentTiles.Add(targetEnemy.npcScript.trueTile + Vector2.right);
        }
        if (TileDataManager.GetTileData(targetEnemy.npcScript.trueTile + Vector2.up).obstacle == false)
        {
            adjacentTiles.Add(targetEnemy.npcScript.trueTile + Vector2.up);
        }
        if (TileDataManager.GetTileData(targetEnemy.npcScript.trueTile + Vector2.down).obstacle == false)
        {
            adjacentTiles.Add(targetEnemy.npcScript.trueTile + Vector2.down);
        }

        distance = 10000;
        targetTile = adjacentTiles[0];
        foreach (Vector2 tile in adjacentTiles)
        {
            List<Vector2> path = pathFinder.FindAStarPath(playerScript.truePlayerTile, tile);
            if (path.Count < distance)
            {
                targetTile = tile;
                distance = path.Count;
            }
        }

        playerScript.trueTile.ExternalMovement(targetTile);




    }
}
