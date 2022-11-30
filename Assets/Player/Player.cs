using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public bool runEnabled;
    [System.NonSerialized] public bool forceWalk = false;
    public GameObject trueTileObject;
    [HideInInspector] public TrueTile trueTileScript;
    [HideInInspector] public Combat combatScript;
    public Vector2 playerPosition;
    float moveSpeed = 3f;
    public List<Vector2> playerPath;

    public Vector2 trueTile;

    public float runEnergy = 500;
    [System.NonSerialized] public float weight = 40;
    [HideInInspector] public bool stamina;
    [HideInInspector] public int staminaTicks;

    [HideInInspector] public bool showTrueTile = false;
    [HideInInspector] public bool showClickedTile = false;

    float angleFacing;
    float targetAngle;
    Transform playerArrow;
    SpriteRenderer arrowColor;
    float rotationSpeed = 300;

    public static NPC targetedNPC;
    public static bool attackTargetedNPC;
    [HideInInspector] public Vector2 targetNPCPreviousTile;
    [HideInInspector] public bool attackThisTick = false;
    [System.NonSerialized] public bool attackUsingSpell = false;
    [System.NonSerialized] public Spell spellBeingUsed;

    public class IncomingDamage
    {
        public int damage;
        public int ticks;
        public Enemy enemyAttacking;
    }
    GameObject newHitSplat;
    List<IncomingDamage> damageQueue = new List<IncomingDamage>();
    GameObject newHealthBar;

    public static Player player;

    public GameObject minimapIcon;

    public bool kandarinHard = true;

    public bool debugEnabled = false;

    bool dead = false;
    public delegate void PlayerEvents();
    public event PlayerEvents playerDeath;

    void Start()
    {
        Action.ignoreAllActions = false;

        Vector2 startTile = TileManager.FindTile(transform.position);
        transform.position = startTile;

        trueTileScript = GetComponent<TrueTile>();
        trueTileScript.player = this;
        trueTileScript.debugEnabled = debugEnabled;
        trueTileScript.showTrueTile = showTrueTile;

        runEnabled = false;

        playerPath = new List<Vector2>();

        playerArrow = transform.GetChild(0);
        arrowColor = playerArrow.GetChild(0).GetComponent<SpriteRenderer>();

        combatScript = GetComponent<Combat>();
        combatScript.onPlayer = true;
        player = this;

        TickManager.beforeTick += BeforeTick;
        TickManager.onTick += OnTick;
        TickManager.afterTick += AfterTick;

        TrueTile.afterMovement += PerformAttack;
    }

    void Update()
    {
        playerPosition = new Vector2(transform.position.x, transform.position.y);
        if (playerPath.Count > 0)
        {
            float trueMoveSpeed = moveSpeed * 0.5f;
            if (runEnabled && forceWalk == false)
            {
                trueMoveSpeed = moveSpeed;
            }

            if (Mathf.Abs(playerPosition.x - playerPath[0].x) > 0 && Mathf.Abs(playerPosition.y - playerPath[0].y) > 0)
            {
                trueMoveSpeed *= Mathf.Sqrt(2);
            }
            if ((playerPosition - trueTileScript.currentTile).magnitude > 2 || playerPath.Count > 2)
            {
                trueMoveSpeed *= 1.3f;
            }

            transform.position = Vector2.MoveTowards(transform.position, playerPath[0], trueMoveSpeed * Time.deltaTime);

            if ((playerPosition - playerPath[0]).magnitude < 0.01f)
            {
                transform.position = playerPath[0];
                playerPath.RemoveAt(0);
                forceWalk = false;
                if (runEnabled && playerPath.Count > 0 && playerPath[0] == trueTileScript.destinationTile && trueTileScript.oddTilesInPath)
                {
                    forceWalk = true;
                }
            }
            else if (targetedNPC == null)
            {
                targetAngle = Tools.VectorToAngle(playerPath[0] - playerPosition);
            }
        }
        if (targetedNPC != null && (targetedNPC.transform.position - transform.position).magnitude > 0.1f)
        {
            targetAngle = Tools.VectorToAngle(targetedNPC.transform.position - transform.position);
        }

        angleFacing = Mathf.MoveTowardsAngle(angleFacing, targetAngle, rotationSpeed * Time.deltaTime);
        playerArrow.transform.eulerAngles = new Vector3(0, 0, angleFacing);

        minimapIcon.transform.localEulerAngles = Camera.main.transform.localEulerAngles;
    }

    void BeforeTick()
    {
        if (targetedNPC != null)
        {
            targetNPCPreviousTile = targetedNPC.trueTile;
        }
    }
    void OnTick()
    {
        if (trueTileScript.moving && runEnabled && forceWalk == false)
        {
            float runDrain = 67 + (67 * Mathf.Clamp(weight, 0, 64) / 64);
            if (stamina)
            {
                runDrain *= 0.3f;
            }
            runEnergy -= runDrain;
            if (runEnergy < 0)
            {
                runEnergy = 0;
                runEnabled = false;
            }
        }
        else
        {
            runEnergy = Mathf.Min(10000, runEnergy + (99 / 6) + 8);
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

        if (targetedNPC != null && attackTargetedNPC)
        {
            AttackEnemy(targetedNPC.GetComponent<Enemy>());
        }
    }
    void AfterTick()
    {
        if (targetedNPC != null)
        {
            if (attackThisTick)
            {
                arrowColor.color = Color.red;
            }
            else
            {
                arrowColor.color = Color.yellow;
            }
        }
        else
        {
            arrowColor.color = Color.green;
        }

        if (staminaTicks > 0)
        {
            staminaTicks--;
        }
        if (stamina && staminaTicks == 0)
        {
            RunToggle.instance.Stamina(false);
            stamina = false;
        }
    }

    public void AddToDamageQueue(int damage, int tickDelay, Enemy enemyAttacking)
    {
        IncomingDamage newDamage = new IncomingDamage();
        newDamage.damage = damage;
        newDamage.ticks = tickDelay;
        newDamage.enemyAttacking = enemyAttacking;
        damageQueue.Add(newDamage);
    }
    public void TakeDamage(IncomingDamage damage)
    {
        if (damage.damage > PlayerStats.currentHitpoints)
        {
            damage.damage = PlayerStats.currentHitpoints;
        }

        PlayerStats.currentHitpoints -= damage.damage;
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
            healthScript.maxHealth = PlayerStats.initialHitpoints;
            healthScript.currentHealth = PlayerStats.currentHitpoints;
            healthScript.objectWithHealth = gameObject;
            healthScript.worldSpaceOffset = 0.5f;
        }
        else
        {
            HealthBar healthScript = newHealthBar.GetComponent<HealthBar>();
            healthScript.UpdateHealth(PlayerStats.currentHitpoints);
        }

        if (AutoRetaliate.active && damage.enemyAttacking != null)
        {
            AttackEnemy(damage.enemyAttacking);
        }

        if (damage.damage > 0 && WornEquipment.recoil && damage.enemyAttacking != null)
        {
            int recoilDamage = Mathf.FloorToInt((float)damage.damage * 0.1f + 1);
            damage.enemyAttacking.AddToDamageQueue(recoilDamage, 0, false, 0);
        }

        if (PlayerStats.currentHitpoints <= 0 && dead == false)
        {
            dead = true;
            Death();
        }
    }

    void Death()
    {
        playerDeath?.Invoke();
        damageQueue = new List<IncomingDamage>();
        Action.ignoreAllActions = true;
    }

    public void AttackEnemy(Enemy enemy)
    {
        targetedNPC = enemy.GetComponent<NPC>();
        attackTargetedNPC = true;
        targetNPCPreviousTile = targetedNPC.trueTile;
        targetAngle = Tools.VectorToAngle(targetedNPC.transform.position - transform.position);
        int range = Mathf.Min(WornEquipment.attackDistance + AttackStyles.distanceBonus, 10);
        if (attackUsingSpell && spellBeingUsed != null)
        {
            range = 10;
        }
        if (combatScript.CapableOfAttacking(trueTile, targetNPCPreviousTile, targetedNPC, range, false))
        {
            if (trueTileScript.moving)
            {
                trueTileScript.StopMovement();
            }
        }
        else
        {
            if (combatScript.AdjacentTileAvailable(targetNPCPreviousTile, enemy.npcScript.tileSize) == false && range == 1)
            {
                Debug.Log("I can't reach that!");
                targetedNPC = null;
                attackTargetedNPC = false;
            }
            else
            {
                Vector2 targetTile = combatScript.FindEnemyAdjacentTile(targetNPCPreviousTile, enemy.npcScript.tileSize);
                trueTileScript.ExternalMovement(targetTile);
            }
        }
    }
    void PerformAttack()
    {
        if (targetedNPC == null || attackTargetedNPC == false)
        {
            attackThisTick = false;
            return;
        }
        int range = Mathf.Min(WornEquipment.attackDistance + AttackStyles.distanceBonus, 10);
        if ((attackUsingSpell && spellBeingUsed != null) || AttackStyles.selectedStyle == 8 || AttackStyles.selectedStyle == 9)
        {
            range = 10;
        }

        if (combatScript.CapableOfAttacking(trueTile, targetNPCPreviousTile, targetedNPC, range, false))
        {
            if (attackUsingSpell && spellBeingUsed != null)
            {
                Spellbook.CountRunes();
                combatScript.PlayerCastSpell(spellBeingUsed);
            }
            else if (AttackStyles.selectedStyle == 8 || AttackStyles.selectedStyle == 9)
            {
                Spellbook.CountRunes();
                combatScript.PlayerCastSpell(AttackStyles.currentSpellOnAutocast);
            }
            else
            {
                combatScript.PlayerAttack();
            }
        }
        else
        {
            attackThisTick = false;
        }
    }
    public void RemoveFocus()
    {
        targetedNPC = null;
        attackTargetedNPC = false;
    }

    public void ClearDamageQueue()
    {
        damageQueue = new List<IncomingDamage>();
    }


}
