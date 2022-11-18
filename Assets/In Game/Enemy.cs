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
    public int attackDistance = 1;
    public string attackStyle;

    public bool aggro;
    public int aggroRange = 3;

    public bool willOverrideMaxHit = false;
    public int newMaxHit;

    public bool ignoreLineOfSight = false;


    public bool typelessAttack = false;
    public int overheadProtectionMult = 1;
    public Color projectileColor;

    [HideInInspector] bool inCombat;

    [HideInInspector] public float combatLevel;
    [HideInInspector] public string combatLevelColor;

    [HideInInspector] public NPC npcScript;
    Action npcAction;

    Player playerScript;
    Combat combatScript;
    [HideInInspector] public bool isAttackingPlayer = false;
    [HideInInspector] public bool attackThisTick = false;
    Vector2 playerPreviousTile;

    GameObject newHealthBar;


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

    void PlayerAttack()
    {
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

        if (combatScript.attackCooldown == combatScript.minCoolDown)
        {
            inCombat = false;
        }
    }

    public void AddToDamageQueue(int damage, int tickDelay, bool fromPlayer, int maxHit)
    {
        IncomingDamage newDamage = new IncomingDamage();
        newDamage.damage = damage;
        newDamage.ticks = tickDelay;
        newDamage.fromPlayer = fromPlayer;
        newDamage.maxHit = maxHit;
        damageQueue.Add(newDamage);
    }
    void TakeDamage(IncomingDamage damage)
    {
        hitpoints -= damage.damage;
        GameObject newHitSplat = Instantiate(UIManager.staticHitSplat, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity);
        HitSplat splatScript = newHitSplat.GetComponent<HitSplat>();
        splatScript.objectGettingHit = gameObject;
        splatScript.showMaxHitSplat = damage.fromPlayer;
        splatScript.NewHitSplat(damage.damage, damage.maxHit);

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

        if (damage.fromPlayer && isAttackingPlayer == false)
        {
            AttackPlayer();
            if (inCombat == false)
            {
                combatScript.attackCooldown = 2;
                inCombat = true;
            }
        }
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
        if (isAttackingPlayer == false || npcScript.isTargetingPlayer == false)
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
}
