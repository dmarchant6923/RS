using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZukHealer : MonoBehaviour
{
    [HideInInspector] public Zuk zukScript;
    Enemy enemyScript;
    NPC npcScript;
    Combat combatScript;

    int healAmountLow = 15;
    int healAmountHigh = 24;
    int healRate = 3;
    int healTicks;

    bool healInterrupted;
    public Sprite projectile;
    public Color projectileColor;
    public GameObject explosion;

    bool addedAttackActionBack;

    public class AOE
    {
        public Vector2 position;
        public int damage;
        public int delay;
    }

    List<AOE> activeAttacks = new List<AOE>();

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();
        npcScript = GetComponent<NPC>();
        combatScript = GetComponent<Combat>();

        npcScript.externalFocus = true;

        healTicks = healRate;

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += AfterTick;
        enemyScript.tookDamage += SwitchAggro;
    }

    void SwitchAggro()
    {
        enemyScript.tookDamage -= SwitchAggro;
        healInterrupted = true;
        combatScript.attackCooldown = 3;
        npcScript.targetAngle = Tools.VectorToAngle(Vector2.down);
        enemyScript.inCombat = true;
        enemyScript.AttackPlayer();
        if (Player.targetedNPC == enemyScript.npcScript)
        {
            Player.player.RemoveFocus();
        }
        enemyScript.npcAction.serverAction0 -= enemyScript.PlayerAttack;
        enemyScript.npcAction.serverAction0 += DummyMethod;
    }

    void DummyMethod()
    {

    }

    void BeforeTick()
    {
        for (int i = 0; i < activeAttacks.Count; i++)
        {
            activeAttacks[i].delay--;
            if (activeAttacks[i].delay == 0)
            {
                if (TileManager.TileDistance(Player.player.trueTile, activeAttacks[i].position) < 2)
                {
                    Player.player.AddToDamageQueue(activeAttacks[i].damage, 0, null);
                }
                Instantiate(explosion, activeAttacks[i].position, Quaternion.identity);

                activeAttacks.RemoveAt(i);
                i--;
            }
        }
    }

    void AfterTick()
    {
        enemyScript.attackThisTick = false;
        if (healInterrupted == false)
        {
            if (healTicks > 0)
            {
                healTicks--;
            }

            if (enemyScript.isAttackingPlayer)
            {
                healInterrupted = true;
            }

            if (healTicks == 0)
            {
                enemyScript.attackThisTick = true;
                int healed = Random.Range(healAmountLow, healAmountHigh + 1);
                zukScript.enemyScript.AddToDamageQueue(-healed, 3, false, 0);
                healTicks = healRate;
                combatScript.SpawnProjectile(zukScript.gameObject, gameObject, 3, projectileColor, projectile);
            }
        }
        else
        {
            if (combatScript.attackCooldown < 2 && addedAttackActionBack == false)
            {
                enemyScript.npcAction.serverAction0 += enemyScript.PlayerAttack;
                enemyScript.npcAction.serverAction0 -= DummyMethod;
            }

            if (combatScript.attackCooldown <= 0)
            {
                enemyScript.attackThisTick = true;
                for (int i = 0; i < 3; i++)
                {
                    AOE newAttack = new AOE();
                    int positionX = (int)npcScript.trueTile.x + Random.Range(-5, 6);
                    int positionY = Random.Range(0, -4);
                    newAttack.position = new Vector2(positionX, positionY);
                    newAttack.damage = Random.Range(5, 11);
                    newAttack.delay = 4;
                    activeAttacks.Add(newAttack);
                    combatScript.SpawnProjectile(newAttack.position, gameObject, 4, projectileColor, projectile);
                }
                combatScript.attackCooldown = 3;
            }
        }
    }

    void Update()
    {
        if (healInterrupted == false)
        {
            npcScript.targetAngle = Tools.VectorToAngle(zukScript.transform.position - transform.position);
        }
    }

    private void OnDestroy()
    {
        TickManager.beforeTick -= BeforeTick;
        TickManager.afterTick -= AfterTick;
    }
}
