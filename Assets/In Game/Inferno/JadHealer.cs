using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JadHealer : MonoBehaviour
{
    public Jad jadScript;
    int healRange = 1;
    int healAmountLow = 15;
    int healAmountHigh = 24;
    int healRate = 4;

    int healTicks;

    bool healing;
    bool healInterrupted;

    NPC npcScript;
    Combat combatScript;
    Enemy enemyScript;

    private void Start()
    {
        healTicks = 4;

        npcScript = GetComponent<NPC>();
        combatScript = GetComponent<Combat>();
        enemyScript = GetComponent<Enemy>();

        npcScript.beforeMovement += BeforeMovement;
        TickManager.afterTick += AfterTick;
        enemyScript.tookDamage += SwitchAggro;
        npcScript.externalTarget = true;

    }

    void BeforeMovement()
    {
        healing = false;
        if (healInterrupted == false)
        {
            int distance = combatScript.DistanceToCombat(transform.position, jadScript.npcScript.trueTile, jadScript.npcScript.tileSize);
            if (distance > healRange)
            {
                npcScript.ExternalMovement(jadScript.npcScript.trueTile);
            }
            else
            {
                healing = true;
            }
        }
    }

    void SwitchAggro()
    {
        healInterrupted = true;
    }

    void AfterTick()
    {
        if (jadScript == null)
        {
            Destroy(gameObject);
        }

        if (healInterrupted)
        {
            return;
        }

        if (healTicks > 0)
        {
            healTicks--;
        }

        if (enemyScript.isAttackingPlayer)
        {
            healInterrupted = true;
        }

        if (healing && healTicks == 0)
        {
            int healed = Random.Range(healAmountLow, healAmountHigh + 1);
            jadScript.enemyScript.AddToDamageQueue(-healed, 0, false, 0);
            healTicks = healRate;
        }
    }

    void Update()
    {
        if (healInterrupted == false)
        {
            npcScript.targetAngle = Tools.VectorToAngle(jadScript.transform.position - transform.position);
        }
    }

    private void OnDestroy()
    {
        TickManager.afterTick -= AfterTick;
    }
}
