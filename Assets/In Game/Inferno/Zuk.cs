using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zuk : MonoBehaviour
{
    public GameObject ranger;
    public GameObject mager;
    public GameObject healer;

    int threshold1 = 600;
    bool thresh1Passed;
    int threshold2 = 480;
    bool thresh2Passed;
    int threshold3 = 240;
    bool thresh3Passed;

    int attackSpeed = 10;

    NPC NPCScript;
    Enemy enemyScript;
    Combat combatScript;
    void Start()
    {
        NPCScript = GetComponent<NPC>();
        enemyScript = GetComponent<Enemy>();
        combatScript = GetComponent<Combat>();
        combatScript.attackCooldown = attackSpeed;
        NPCScript.isTargetingPlayer = true;

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += AfterTick;
    }

    void BeforeTick()
    {
        enemyScript.attackThisTick = false;
        if (combatScript.attackCooldown <= 0)
        {
            enemyScript.AttackPlayer();
        }

        if (enemyScript.hitpoints < threshold1 && thresh1Passed == false)
        {
            thresh1Passed = true;
        }
        if (enemyScript.hitpoints < threshold2 && thresh2Passed == false)
        {
            thresh2Passed = true;
        }
        if (enemyScript.hitpoints < threshold3 && thresh3Passed == false)
        {
            thresh3Passed = true;
        }
    }

    void AfterTick()
    {
        enemyScript.StopAttacking();
    }
}
