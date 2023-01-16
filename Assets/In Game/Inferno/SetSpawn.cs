using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawn : MonoBehaviour
{
    public ZukShield shieldScript;
    NPC npcScript;
    Combat combatScript;
    Enemy enemyScript;

    bool attackPlayer = false;

    public int attackSpeed;

    public bool Jad = false;
    public bool switchAggroOnAttack = false;


    IEnumerator Start()
    {
        npcScript = GetComponent<NPC>();
        combatScript = GetComponent<Combat>();
        enemyScript = GetComponent<Enemy>();
        enemyScript.inCombat = true;
        npcScript.externalFocus = true;
        enemyScript.aggro = false;

        yield return null;

        enemyScript.isAttackingPlayer = false;
        enemyScript.attackSpeed = attackSpeed;
        combatScript.attackCooldown = 5;
        if (Jad)
        {
            combatScript.attackCooldown = 9;
        }

        TickManager.beforeTick += BeforeTick;
        enemyScript.tookDamage += TookDamage;
    }

    void BeforeTick()
    {
        enemyScript.attackThisTick = false;
        if (attackPlayer == false && (shieldScript == null || shieldScript.enemyScript.death))
        {
            SwitchAggro();
        }

        if (attackPlayer == false && enemyScript.isAttackingPlayer)
        {
            TookDamage();
        }

        if (attackPlayer == false && combatScript.attackCooldown <= 0)
        {
            enemyScript.attackThisTick = true;
            int delay = 4;
            if (Jad)
            {
                delay = 2;
            }
            combatScript.SpawnProjectile(shieldScript.gameObject, gameObject, delay, enemyScript.projectileColor, "");
            int hitRoll = Random.Range(0, (int)combatScript.EnemyMaxHit(enemyScript) + 1);
            shieldScript.enemyScript.AddToDamageQueue(hitRoll, delay, false, 0);
            combatScript.attackCooldown = attackSpeed;
            if (switchAggroOnAttack)
            {
                SwitchAggro();
            }
        }
    }

    void TookDamage()
    {
        if (attackPlayer || switchAggroOnAttack)
        {
            return;
        }
        //if (Jad && combatScript.attackCooldown <= 3)
        //{
        //    switchAggroOnAttack = true;
        //}
        else
        {
            SwitchAggro();
        }
    }

    void SwitchAggro()
    {
        enemyScript.tookDamage -= TookDamage;
        if (attackPlayer)
        {
            return;
        }
        switchAggroOnAttack = false;
        attackPlayer = true;
        combatScript.attackCooldown = 2;
        if (Jad)
        {
            combatScript.attackCooldown = Mathf.Min(5, combatScript.attackCooldown);
        }
        npcScript.externalFocus = false;
        enemyScript.AttackPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackPlayer == false && shieldScript != null)
        {
            npcScript.targetAngle = Tools.VectorToAngle(shieldScript.transform.position - transform.position);
        }
    }
}
