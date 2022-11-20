using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zuk : MonoBehaviour
{
    public GameObject rangerPrefab;
    public GameObject magerPrefab;
    public GameObject jadPrefab;
    public GameObject healerPrefab;

    public Transform mageSpawnTile;
    public Transform rangeSpawnTile;
    public Transform jadSpawnTile;
    public Transform[] healerSpawnTiles = new Transform[4];

    int threshold1 = 600;
    bool thresh1Passed;
    int threshold2 = 480; //480;
    bool thresh2Passed;
    int threshold3 = 240;
    bool thresh3Passed;

    NPC NPCScript;
    Enemy enemyScript;
    Combat combatScript;
    public ZukShield shieldScript;

    int timer = 75; //75
    bool timerPaused = false;

    IEnumerator Start()
    {
        NPCScript = GetComponent<NPC>();
        enemyScript = GetComponent<Enemy>();
        combatScript = GetComponent<Combat>();
        NPCScript.isTargetingPlayer = true;

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += AfterTick;

        mageSpawnTile.gameObject.SetActive(false);
        rangeSpawnTile.gameObject.SetActive(false);
        jadSpawnTile.gameObject.SetActive(false);
        foreach (Transform healer in healerSpawnTiles)
        {
            healer.gameObject.SetActive(false);
        }

        yield return null;

        combatScript.attackCooldown = 13;
    }

    void BeforeTick()
    {
        enemyScript.attackThisTick = false;
        if (combatScript.attackCooldown <= 0)
        {
            if (Player.player.trueTile.x >= shieldScript.safeSpotRange[0] && Player.player.trueTile.x <= shieldScript.safeSpotRange[1] && 
                Player.player.trueTile.y >= -2 && shieldScript != null)
            {
                combatScript.attackCooldown = enemyScript.attackSpeed + 1;
                combatScript.SpawnProjectile(shieldScript.gameObject, gameObject, 5, enemyScript.projectileColor, "");
                enemyScript.attackThisTick = true;
            }
            else
            {
                enemyScript.AttackPlayer();
            }
        }

        if (enemyScript.hitpoints < threshold1 && thresh1Passed == false)
        {
            thresh1Passed = true;
            timerPaused = true;
        }
        if (enemyScript.hitpoints < threshold2 && thresh2Passed == false)
        {
            thresh2Passed = true;
            timerPaused = false;
            timer += 175;
            JadSpawn();
        }
        if (enemyScript.hitpoints < threshold3 && thresh3Passed == false)
        {
            thresh3Passed = true;
        }

        if (timerPaused == false)
        {
            timer--;
            if (timer <= 0)
            {
                timer = 350;
                SetSpawn();
            }
        }
    }

    void AfterTick()
    {
        enemyScript.StopAttacking();
    }

    void SetSpawn()
    {
        GameObject newRanger = Instantiate(rangerPrefab, rangeSpawnTile.position + Vector3.one * 1.5f, Quaternion.identity);
        newRanger.name = rangerPrefab.name;
        SetSpawn spawnScript = newRanger.AddComponent<SetSpawn>();
        spawnScript.shieldScript = shieldScript;
        spawnScript.attackSpeed = 4;

        GameObject newMager = Instantiate(magerPrefab, mageSpawnTile.position + Vector3.one * 2f, Quaternion.identity);
        newMager.name = magerPrefab.name;
        spawnScript = newMager.AddComponent<SetSpawn>();
        spawnScript.shieldScript = shieldScript;
        spawnScript.attackSpeed = 4;
    }

    void JadSpawn()
    {
        GameObject newJad = Instantiate(jadPrefab, jadSpawnTile.position + Vector3.one * 2.5f, Quaternion.identity);
        newJad.name = jadPrefab.name;
        SetSpawn spawnScript = newJad.AddComponent<SetSpawn>();
        spawnScript.shieldScript = shieldScript;
        spawnScript.Jad = true;
        spawnScript.attackSpeed = 8;
    }
}
