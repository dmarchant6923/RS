using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hitpoints = 10;
    [HideInInspector] public int initialHitpoints;

    public int attack = 1;
    public int strength = 1;
    public int defence = 1;
    int initialDefence;
    public int magic = 1;
    public int ranged = 1;

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
    public int attackDistance = 1;
    public string attackStyle;
    public bool useMeleeUpClose;
    public string meleeStyle = "Slash";
    public float percentMeleeUpClose;

    public bool autoRetaliate = true;
    public bool aggro;
    public int aggroRange = 3;

    public bool willOverrideMaxHit = false;
    public int newMaxHit;

    public bool ignoreLineOfSight = false;
    public bool ignoreAccuracyCheck = false;

    public bool typelessAttack = false;
    public bool overkill = false;
    public int overheadProtectionMult = 1;
    public Color projectileColor;
    public bool slowProjectile = false;
    public int uniformProjectileDelay = 0;
    public Sprite customProjectile;

    public int customCombatLvl;

    public float xpMult = 1;

    [HideInInspector] public bool inCombat;

    [HideInInspector] public float combatLevel;
    [HideInInspector] public string combatLevelColor;

    [HideInInspector] public NPC npcScript;
    [HideInInspector] public Action npcAction;

    Player playerScript;
    Combat combatScript;
    [HideInInspector] public bool isAttackingPlayer = false;
    [HideInInspector] public bool attackThisTick = false;
    Vector2 playerPreviousTile;

    GameObject newHealthBar;
    GameObject newHitSplat;

    [HideInInspector] public bool death;
    int deathTicks = 4;

    public delegate void EnemyCombat();
    public event EnemyCombat tookDamage;
    public event EnemyCombat beforeAttack;
    public event EnemyCombat enemyDied;


    public class IncomingDamage
    {
        public int damage;
        public int ticks;
        public bool fromPlayer = false;
        public int maxHit;
    }
    List<IncomingDamage> damageQueue = new List<IncomingDamage>();

    bool playerAttackingEnemy = false;

    private IEnumerator Start()
    {
        initialHitpoints = hitpoints;
        initialDefence = defence;

        npcScript = GetComponent<NPC>();
        npcAction = GetComponent<Action>();

        playerScript = FindObjectOfType<Player>();
        combatScript = GetComponent<Combat>();
        combatScript.attackCooldown = attackSpeed;

        float baselvl = 0.25f * ((float)defence + hitpoints + (1 * 0.5f));
        float meleelvl = (13f / 40f) * ((float)attack + strength);
        float rangelvl = (13f / 40f) * ((float)ranged * 3 / 2);
        float magelvl = (13f / 40f) * ((float)magic * 3 / 2);
        combatLevel = baselvl + Mathf.Max(meleelvl, rangelvl, magelvl);

        if (customCombatLvl > 0)
        {
            combatLevel = customCombatLvl;
        }

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

        if (attackStyle.ToLower().Contains("slash"))
        {
            attackStyle = AttackStyles.slashStyle;
            useMeleeUpClose = false;
        }
        else if (attackStyle.ToLower().Contains("stab"))
        {
            attackStyle = AttackStyles.stabStyle;
            useMeleeUpClose = false;
        }
        else if (attackStyle.ToLower().Contains("crush"))
        {
            attackStyle = AttackStyles.crushStyle;
            useMeleeUpClose = false;
        }
        else if (attackStyle.ToLower().Contains("range"))
        {
            attackStyle = AttackStyles.rangedStyle;
        }
        else if (attackStyle.ToLower().Contains("mag"))
        {
            attackStyle = AttackStyles.magicStyle;
        }

        yield return null;

        npcScript.menuTexts[0] = "Attack ";
        npcAction.cancelLevels[0] = 1;
        npcAction.menuPriorities[0] = 1;
        npcAction.staticPlayerActions[0] = true;
        npcAction.serverAction0 += PlayerAttack;
        npcScript.UpdateActions(gameObject.name, true);

        Action.cancel1 += CancelPlayerAttack;

        TickManager.beforeTick += BeforeTick;
        TickManager.onTick += PerformAttack;

        npcScript.beforeMovement += BeforeMovement;
    }

    public void PlayerAttack()
    {
        if (death)
        {
            return;
        }
        playerAttackingEnemy = true;
        playerScript.AttackEnemy(this);
    }
    void CancelPlayerAttack()
    {
        playerAttackingEnemy = false;
        if (Player.targetedNPC == npcScript)
        {
            Player.targetedNPC = null;
        }
    }

    void BeforeTick()
    {
        if (death)
        {
            deathTicks--;
            if (deathTicks == 0)
            {
                Destroy(gameObject);
            }
            return;
        }

        foreach (IncomingDamage damage in damageQueue)
        {
            damage.ticks--;
            if (damage.ticks <= 0)
            {
                TakeDamage(damage);
            }
        }
        for (int i = 0; i < damageQueue.Count; i++)
        {
            if (damageQueue[i].ticks <= 0)
            {
                damageQueue.Remove(damageQueue[i]);
                i--;
            }
        }

        if (combatScript.attackCooldown == Combat.minCoolDown)
        {
            inCombat = false;
        }
    }

    public void AddToDamageQueue(int damage, int tickDelay, bool fromPlayer, int maxHit)
    {
        if (death)
        {
            return;
        }

        IncomingDamage newDamage = new IncomingDamage();
        newDamage.damage = damage;
        newDamage.ticks = tickDelay;
        newDamage.fromPlayer = fromPlayer;
        newDamage.maxHit = maxHit;
        damageQueue.Add(newDamage);
    }
    void TakeDamage(IncomingDamage damage)
    {
        if (damage.damage > hitpoints)
        {
            damage.damage = hitpoints;
        }

        hitpoints -= damage.damage;
        if (hitpoints > initialHitpoints)
        {
            hitpoints = initialHitpoints;
        }

        if (newHitSplat == null)
        {
            newHitSplat = Instantiate(UIManager.staticHitSplat, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity);
            HitSplat splatScript = newHitSplat.GetComponent<HitSplat>();
            splatScript.NewHitSplat(damage.damage, 0);
            splatScript.objectGettingHit = gameObject;
        }
        else
        {
            newHitSplat.GetComponent<HitSplat>().NewHitSplat(damage.damage, 0);
        }


        if (newHealthBar == null)
        {
            newHealthBar = Instantiate(UIManager.staticHealthBar, transform.position, Quaternion.identity);
            HealthBar healthScript = newHealthBar.GetComponent<HealthBar>();
            healthScript.maxHealth = initialHitpoints;
            healthScript.currentHealth = hitpoints;
            healthScript.objectWithHealth = gameObject;
            healthScript.worldSpaceOffset = 0.5f;
        }
        else
        {
            HealthBar healthScript = newHealthBar.GetComponent<HealthBar>();
            healthScript.UpdateHealth(hitpoints);
        }

        if (damage.fromPlayer && isAttackingPlayer == false && autoRetaliate)
        {
            AttackPlayer();
            if (inCombat == false)
            {
                combatScript.attackCooldown = 2;
                inCombat = true;
            }
        }

        tookDamage?.Invoke();

        if (hitpoints <= 0)
        {
            Death();
        }
    }
    public void ClearDamageQueue()
    {
        damageQueue = new List<IncomingDamage>();
    }

    void Death()
    {
        enemyDied?.Invoke();
        death = true;
        hitpoints = 0;
        isAttackingPlayer = false;
        npcScript.isTargetingPlayer = false;
        npcScript.StopMovement();
        damageQueue = new List<IncomingDamage>();
        CancelPlayerAttack();
        TickManager.onTick -= PerformAttack;
    }

    public void SpellEffects(Spell spell)
    {
        if (spell.freeze)
        {
            if (npcScript.freezeTicks < -4)
            {
                npcScript.freezeTicks = spell.freezeLength;
            }
        }
    }

    public void BeforeMovement()
    {
        if (npcScript.isTargetingPlayer && isAttackingPlayer)
        {
            AttackPlayer();
        }
        else if (aggro && TileManager.TileDistance(playerScript.trueTile, npcScript.trueTile) <= aggroRange)
        {
            AttackPlayer();
        }
    }

    public void AttackPlayer()
    {
        if (npcScript == null || Player.player.standardDeath)
        {
            return;
        }

        beforeAttack?.Invoke();
        npcScript.isTargetingPlayer = true;
        npcScript.externalTarget = true;
        isAttackingPlayer = true;
        playerPreviousTile = playerScript.trueTile;
        if (combatScript.CapableOfAttacking(playerPreviousTile, npcScript.trueTile, npcScript, attackDistance, ignoreLineOfSight))
        {
            npcScript.StopMovement();
        }
        else if (combatScript.PlayerInsideEnemy(this))
        {
            int randX = Random.Range(0, (int)2);
            int randY = Random.Range(0, (int)5);
            Vector2 target = Vector2.zero;
            if (randX == 0 && randY != 4)
            {
                target = new Vector2(Random.Range(0, (int)2) * 2 - 1, 0);
            }
            else if (randY != 4)
            {
                target = new Vector2(0, Random.Range(0, (int)2) * 2 - 1);
            }

            npcScript.ExternalMovement(npcScript.trueTile + target);
        }
        else
        {
            Vector2 targetTile = playerScript.trueTile;
            if (combatScript.DistanceToCombat(playerScript.trueTile, npcScript.trueTile, npcScript.tileSize) == 1)
            {
                targetTile = combatScript.FindPlayerAdjacentTile(npcScript.trueTile, playerScript.trueTile, npcScript.tileSize);
            }
            npcScript.ExternalMovement(targetTile);
        }
    }
    void PerformAttack()
    {
        if (npcScript == null || isAttackingPlayer == false || npcScript.isTargetingPlayer == false || death || Player.player.standardDeath)
        {
            return;
        }

        if (combatScript.CapableOfAttacking(playerPreviousTile, npcScript.trueTile, npcScript, attackDistance, ignoreLineOfSight))
        {
            combatScript.EnemyAttack(this);
        }
        else
        {
            attackThisTick = false;
        }
    }

    public void StopAttacking()
    {
        isAttackingPlayer = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= CancelPlayerAttack;
        TickManager.beforeTick -= BeforeTick;
        TickManager.onTick -= PerformAttack;
    }
}
