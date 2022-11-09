using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public bool runEnabled;
    [System.NonSerialized] public bool forceWalk = false;
    public GameObject trueTileObject;
    GameObject newTrueTileObject;
    [HideInInspector] public TrueTile trueTileScript;
    [HideInInspector] public Combat combatScript;
    public Vector2 playerPosition;
    float moveSpeed = 3f;
    public List<Vector2> playerPath;

    public Vector2 trueTile;

    public float runEnergy = 500;
    [System.NonSerialized] public float weight = 40;

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

    public bool debugEnabled = false;

    void Start()
    {
        Vector2 startTile = TileManager.FindTile(transform.position);
        transform.position = startTile;

        trueTileScript = GetComponent<TrueTile>();
        trueTileScript.player = this;
        trueTileScript.debugEnabled = debugEnabled;
        trueTileScript.showTrueTile = showTrueTile;
        trueTileScript.showClickedTile = showClickedTile;

        runEnabled = false;

        playerPath = new List<Vector2>();

        playerArrow = transform.GetChild(0);
        arrowColor = playerArrow.GetChild(0).GetComponent<SpriteRenderer>();

        combatScript = GetComponent<Combat>();

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
        if (targetedNPC != null)
        {
            targetAngle = Tools.VectorToAngle(targetedNPC.transform.position - transform.position);
        }

        angleFacing = Mathf.MoveTowardsAngle(angleFacing, targetAngle, rotationSpeed * Time.deltaTime);
        playerArrow.transform.eulerAngles = new Vector3(0, 0, angleFacing);
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
            runEnergy -= 67 + (67 * Mathf.Clamp(weight, 0, 64) / 64);
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
    }

    public void AttackEnemy(Enemy enemy)
    {
        targetedNPC = enemy.GetComponent<NPC>();
        attackTargetedNPC = true;
        targetNPCPreviousTile = targetedNPC.trueTile;
        targetAngle = Tools.VectorToAngle(targetedNPC.transform.position - transform.position);
        if (combatScript.InAttackRange(trueTile, targetNPCPreviousTile, WornEquipment.attackDistance))
        {
            trueTileScript.StopMovement();
        }
        else
        {
            if (combatScript.AdjacentTileAvailable(targetNPCPreviousTile) == false)
            {
                Debug.Log("I can't reach that!");
                targetedNPC = null;
                attackTargetedNPC = false;
            }
            else
            {
                Vector2 targetTile = combatScript.FindAdjacentTile(targetNPCPreviousTile);
                trueTileScript.ExternalMovement(targetTile);
            }
        }
    }

    void PerformAttack()
    {
        if (targetedNPC == null || attackTargetedNPC == false)
        {
            return;
        }

        if (combatScript.InAttackRange(trueTile, targetNPCPreviousTile, Mathf.Min(WornEquipment.attackDistance + AttackStyles.distanceBonus, 10)))
        {
            combatScript.PlayerAttack();
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
}
