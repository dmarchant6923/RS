using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    Vector2 originTile;
    [HideInInspector] public Vector2 trueTile;
    public int wanderRange;
    Pathfinder pathFinder;
    int pathTick = 0;
    int newPathTick = 0;
    public int newPathFrequency = 7;

    public int tileSize = 1;
    Vector2 centerOffset;

    List<Vector2> path = new List<Vector2>();
    List<Vector2> npcPath = new List<Vector2>();
    Vector2 npcPosition;
    float moveSpeed = 3.3f;
    public bool runEnabled = false;
    [System.NonSerialized] public bool forceWalk = false;
    [HideInInspector] public bool moving = false;
    public bool stationary = false;

    public static bool showTrueTile = true;
    public bool showSizeTile = false;
    public GameObject trueTileMarker;
    GameObject newTrueTileMarker;
    [HideInInspector] public GameObject newSizeTileMarker;

    [HideInInspector] public Action npcAction;
    [System.NonSerialized] public string[] menuTexts = new string[9];

    [HideInInspector] public bool isTargetingPlayer;
    float angleFacing;
    [HideInInspector] public float targetAngle;
    Transform npcArrow;
    SpriteRenderer arrowColor;
    float rotationSpeed = 300;

    [HideInInspector] public bool externalTarget;
    [HideInInspector] public bool externalFocus;

    public delegate void TrueTileMoved();
    public event TrueTileMoved beforeMovement;
    public event TrueTileMoved afterMovement;

    [HideInInspector] public int freezeTicks;
    [HideInInspector] public bool frozen = false;

    public Transform minimapIcon;

    void Start()
    {
        centerOffset = -Vector2.one * ((float)tileSize - 1) * 0.5f;
        Vector2 center = new Vector2(transform.position.x + centerOffset.x, transform.position.y + centerOffset.y);
        trueTile = TileManager.FindTile(center);
        originTile = trueTile;
        transform.position = trueTile - centerOffset;
        npcPosition = transform.position;
        pathFinder = FindObjectOfType<Pathfinder>();
        pathFinder.debugEnabled = true;
        newPathTick = Random.Range(0, newPathFrequency);

        newTrueTileMarker = Instantiate(trueTileMarker, trueTile, Quaternion.identity);
        Destroy(newTrueTileMarker.GetComponent<BoxCollider2D>());
        newSizeTileMarker = Instantiate(trueTileMarker, trueTile - centerOffset, Quaternion.identity);
        newSizeTileMarker.transform.localScale *= tileSize;
        newTrueTileMarker.GetComponent<SpriteRenderer>().enabled = false;
        if (showTrueTile == false)
        {
            newSizeTileMarker.GetComponent<SpriteRenderer>().enabled = false;
        }

        //if (showSizeTile == false)
        //{
        //}

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += AfterTick;

        npcAction = GetComponent<Action>();
        npcAction.addObjectName = true;
        npcAction.objectName = "<color=yellow>" + gameObject.name + "</color>";
        menuTexts[8] = "Examine ";
        UpdateActions();

        npcArrow = transform.GetChild(0);
        arrowColor = npcArrow.GetChild(0).GetComponent<SpriteRenderer>();
        freezeTicks = -5;

        minimapIcon.localScale *= 1 /transform.localScale.x;

        angleFacing = npcArrow.eulerAngles.z;
        targetAngle = angleFacing;
    }

    public void UpdateActions(string name, bool enemy)
    {
        npcAction.objectName = "<color=yellow>" + name + "</color>";
        if (enemy)
        {
            Enemy enemyScript = GetComponent<Enemy>();
            npcAction.objectName += "<color=" + enemyScript.combatLevelColor + ">  (level-" + Mathf.FloorToInt(GetComponent<Enemy>().combatLevel) + ")</color>";
        }
        UpdateActions();
    }
    public void UpdateActions()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            npcAction.menuTexts[i] = menuTexts[i];
        }
        npcAction.UpdateName();
    }

    public void StopMovement()
    {
        path = new List<Vector2>();
        moving = false;
    }

    public void ExternalMovement(Vector2 destination)
    {
        if (stationary)
        {
            return;
        }
        path = new List<Vector2>();
        path = pathFinder.FindNPCPath(this, trueTile, destination, tileSize);
    }

    void BeforeTick()
    {
        if (externalTarget)
        {

        }
        else
        {
            //stuff for periodic random NPC movement
            if (stationary == false && moving == false && wanderRange > 0)
            {
                pathTick++;
            }
            if (pathTick > newPathTick)
            {
                pathTick = 0;
                newPathTick = Random.Range(Mathf.RoundToInt(newPathFrequency / 2), Mathf.RoundToInt(newPathFrequency * 1.5f));
                path = pathFinder.FindNPCPath(this, trueTile, TileManager.FindTile(originTile + new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange))), tileSize);
            }
        }

        if (freezeTicks > 0)
        {
            frozen = true;
            freezeTicks--;
        }
        else
        {
            frozen = false;
            if (freezeTicks >= -4)
            {
                freezeTicks--;
            }
        }

        beforeMovement?.Invoke();

        if (stationary == false && path.Count > 0 && frozen == false)
        {
            moving = true;
            int i = 1;
            if (runEnabled) { i = 2; }

            while (i > 0)
            {
                if (TileDataManager.GetTileData(path[0]).obstacle)
                {
                    path = new List<Vector2>();
                    moving = false;
                    break;
                }
                trueTile = path[0];
                newTrueTileMarker.transform.position = trueTile;
                newSizeTileMarker.transform.position = trueTile - centerOffset;
                npcPath.Add(trueTile);
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    moving = false;
                    break;
                }
                i--;
            }
        }

        afterMovement?.Invoke();
    }

    void AfterTick()
    {
        if (GetComponent<Enemy>() != null && GetComponent<Enemy>().attackThisTick)
        {
            arrowColor.color = Color.red;
        }
        else if (isTargetingPlayer || externalFocus)
        {
            arrowColor.color = Color.yellow;
        }
        else
        {
            arrowColor.color = Color.green;
        }
    }

    private void Update()
    {
        if (npcPath.Count > 0)
        {
            npcPosition = new Vector2(transform.position.x, transform.position.y) + centerOffset;
            float trueMoveSpeed = moveSpeed * 0.5f;
            if (runEnabled && forceWalk == false)
            {
                trueMoveSpeed = moveSpeed;
            }

            if (Mathf.Abs(npcPosition.x - npcPath[0].x) > 0 && Mathf.Abs(npcPosition.y - npcPath[0].y) > 0)
            {
                trueMoveSpeed *= Mathf.Sqrt(2);
            }
            if ((npcPosition - trueTile).magnitude > 1.5f || npcPath.Count > 2)
            {
                trueMoveSpeed *= 1.3f;
            }

            transform.position = Vector2.MoveTowards(transform.position, npcPath[0] - centerOffset, trueMoveSpeed * Time.deltaTime);

            if ((npcPosition - npcPath[0]).magnitude < 0.01f)
            {
                transform.position = npcPath[0] - centerOffset;
                npcPath.RemoveAt(0);
                forceWalk = false;
            }
            else
            {
                if (isTargetingPlayer == false && externalFocus == false)
                {
                    targetAngle = Tools.VectorToAngle(npcPath[0] - npcPosition);
                }
            }
        }

        if (isTargetingPlayer && (Player.player.transform.position - transform.position).magnitude > 0.1f && externalFocus == false)
        {
            targetAngle = Tools.VectorToAngle(Player.player.transform.position - transform.position);
        }

        angleFacing = Mathf.MoveTowardsAngle(angleFacing, targetAngle, rotationSpeed * Time.deltaTime);
        npcArrow.transform.eulerAngles = new Vector3(0, 0, angleFacing);
    }

    private void OnDestroy()
    {
        TickManager.beforeTick -= BeforeTick;
        TickManager.afterTick -= AfterTick;
        Destroy(newSizeTileMarker);
        Destroy(newTrueTileMarker);
    }
}
