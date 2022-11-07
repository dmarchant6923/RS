using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrueTile : MonoBehaviour
{
    public GameObject clickedTile;
    GameObject newClickedTile;
    public GameObject debugTile;
    GameObject newDebugTile;
    List<GameObject> debugTiles = new List<GameObject>();

    public Player player;
    Pathfinder pathFinder;

    public Vector2 currentTile;
    public Vector2 destinationTile;
    List<Vector2> path;
    public bool moving;
    [HideInInspector] public bool oddTilesInPath = false;

    [HideInInspector] public bool showTrueTile = false;
    [HideInInspector] public bool showClickedTile = false;
    [HideInInspector] public bool debugEnabled = false;

    public delegate void TrueTileMoved();
    public static event TrueTileMoved afterMovement;

    Action walkHereAction;

    void Start()
    {
        TickManager.onTick += Move;

        pathFinder = FindObjectOfType<Pathfinder>();
        pathFinder.debugEnabled = debugEnabled;

        if (showTrueTile == false)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        currentTile = transform.position;
        moving = false;
        path = new List<Vector2>();

        walkHereAction = GetComponent<Action>();
        walkHereAction.clientAction0 += ClientClick;
        walkHereAction.serverAction0 += ServerClick;
        walkHereAction.menuTexts[0] = "walk here";
        walkHereAction.cancelLevels[0] = 1;
        walkHereAction.inGame = true;
        walkHereAction.redClick = false;
    }

    private void Update()
    {
        if (MouseManager.isOverGame && RightClickMenu.tileActions.Contains(walkHereAction) == false)
        {
            RightClickMenu.tileActions.Add(walkHereAction);
        }
        else if (MouseManager.isOverGame == false && RightClickMenu.tileActions.Contains(walkHereAction))
        {
            RightClickMenu.tileActions.Remove(walkHereAction);
        }
    }

    void ClientClick()
    {
        destinationTile = MouseManager.mouseCoordinate;
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        if (showClickedTile)
        {
            newClickedTile = Instantiate(clickedTile, destinationTile, Quaternion.identity);
        }
    }
    public void ClientClick(Vector2 coordinate)
    {
        destinationTile = coordinate;
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        if (showClickedTile)
        {
            newClickedTile = Instantiate(clickedTile, destinationTile, Quaternion.identity);
        }
    }

    public void ExternalMovement(Vector2 coordinate)
    {
        destinationTile = coordinate;
        ServerClick();
    }
    void ServerClick()
    {
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        if (showClickedTile)
        {
            newClickedTile = Instantiate(clickedTile, destinationTile, Quaternion.identity);
        }


        path = pathFinder.FindAStarPath(currentTile, destinationTile);


        if (showClickedTile)
        {
            newClickedTile.transform.position = path[path.Count - 1];
        }
        oddTilesInPath = false;
        if (path.Count % 2 == 1)
        {
            oddTilesInPath = true;
        }
        if (path.Count == 1)
        {
            player.forceWalk = true;
        }
        moving = true;

        if (debugEnabled)
        {
            foreach (GameObject tile in debugTiles)
            {
                Destroy(tile);
            }
            debugTiles = new List<GameObject>();

            foreach (Vector2 tile in path)
            {
                newDebugTile = Instantiate(debugTile, tile, Quaternion.identity);
                debugTiles.Add(newDebugTile);
            }
        }
    }


    //updates on every tick
    void Move()
    {
        if (path.Count > 0)
        {
            int i = 1;
            if (player.runEnabled) { i = 2; }

            while (i > 0)
            {
                currentTile = path[0];
                transform.position = currentTile;
                player.playerPath.Add(currentTile);
                player.truePlayerTile = currentTile;
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    moving = false;
                    if (newClickedTile != null)
                    {
                        Destroy(newClickedTile);
                    }
                    break;
                }
                i--;
            }

        }
        else
        {
            if (debugEnabled)
            {
                foreach (GameObject tile in debugTiles)
                {
                    Destroy(tile);
                }
                debugTiles = new List<GameObject>();
            }
        }

        afterMovement();
    }

    public void StopMovement()
    {
        path = new List<Vector2>();
        moving = false;
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
    }
}
