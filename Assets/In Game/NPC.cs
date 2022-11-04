using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    Vector2 originTile;
    [HideInInspector] public Vector2 trueTile;
    public int wanderRange;
    Pathfinder pathFinder;
    int tick = 0;
    int newPathTick = 0;
    public int newPathFrequency = 7;

    List<Vector2> path = new List<Vector2>();
    List<Vector2> npcPath = new List<Vector2>();
    Vector2 npcPosition;
    float moveSpeed = 3f;
    public bool runEnabled = false;
    [System.NonSerialized] public bool forceWalk = false;
    bool moving = false;

    public bool showTrueTile = false;
    public GameObject trueTileMarker;
    GameObject newTrueTileMarker;

    [HideInInspector] public Action npcAction;
    [System.NonSerialized] public string[] menuTexts = new string[9];

    bool isTargetingPlayer;
    float angleFacing;
    float targetAngle;
    Transform npcArrow;
    float rotationSpeed = 300;

    void Start()
    {
        trueTile = TileManager.FindTile(transform.position);
        originTile = trueTile;
        transform.position = trueTile;
        npcPosition = trueTile;
        pathFinder = FindObjectOfType<Pathfinder>();
        pathFinder.debugEnabled = true;
        newPathTick = Random.Range(0, newPathFrequency);
        if (showTrueTile)
        {
            newTrueTileMarker = Instantiate(trueTileMarker, trueTile, Quaternion.identity);
        }

        TickManager.onTick += OnTick;

        npcAction = GetComponent<Action>();
        npcAction.addObjectName = true;
        npcAction.objectName = "<color=yellow>" + gameObject.name + "</color>";
        menuTexts[8] = "Examine ";
        UpdateActions();

        npcArrow = transform.GetChild(0);
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

    void OnTick()
    {
        if (moving == false && wanderRange > 0)
        {
            tick++;
        }
        if (tick > newPathTick)
        {
            tick = 0;
            newPathTick = Random.Range(Mathf.RoundToInt(newPathFrequency / 2), Mathf.RoundToInt(newPathFrequency * 1.5f));
            path = pathFinder.FindSimplePath(trueTile, TileManager.FindTile(originTile + new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange))));
        }

        if (path.Count > 0)
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
                if (showTrueTile)
                {
                    newTrueTileMarker.transform.position = trueTile;
                }
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
    }

    private void Update()
    {
        if (npcPath.Count > 0)
        {
            npcPosition = new Vector2(transform.position.x, transform.position.y);
            float trueMoveSpeed = moveSpeed * 0.5f;
            if (runEnabled && forceWalk == false)
            {
                trueMoveSpeed = moveSpeed;
            }

            if (Mathf.Abs(npcPosition.x - npcPath[0].x) > 0 && Mathf.Abs(npcPosition.y - npcPath[0].y) > 0)
            {
                trueMoveSpeed *= Mathf.Sqrt(2);
            }
            if ((npcPosition - trueTile).magnitude > 2 || npcPath.Count > 2)
            {
                trueMoveSpeed *= 1.3f;
            }

            transform.position = Vector2.MoveTowards(transform.position, npcPath[0], trueMoveSpeed * Time.deltaTime);
            if (isTargetingPlayer == false)
            {
                targetAngle = Tools.VectorToAngle(npcPath[0] - npcPosition);
            }

            if ((npcPosition - npcPath[0]).magnitude < 0.01f)
            {
                transform.position = npcPath[0];
                npcPath.RemoveAt(0);
                forceWalk = false;
            }
        }

        angleFacing = Mathf.MoveTowardsAngle(angleFacing, targetAngle, rotationSpeed * Time.deltaTime);
        npcArrow.transform.eulerAngles = new Vector3(0, 0, angleFacing);
    }
}
