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

    [HideInInspector] public Player player;
    Pathfinder pathFinder;
    public Transform tileMarker;
    Transform newTileMarker;

    [HideInInspector] public Vector2 currentTile;
    [HideInInspector] public Vector2 destinationTile;
    [HideInInspector] public Vector2 serverDestinationTile;
    List<Vector2> path;
    [HideInInspector] public bool moving;
    [HideInInspector] public bool oddTilesInPath = false;

    [HideInInspector] public bool showTrueTile = false;
    [HideInInspector] public bool showClickedTile = false;
    [HideInInspector] public bool debugEnabled = false;

    public delegate void TrueTileMoved();
    public static event TrueTileMoved afterMovement;

    [HideInInspector] public Action walkHereAction;

    IEnumerator Start()
    {
        TickManager.onTick += Move;

        walkHereAction = GetComponent<Action>();
        pathFinder = FindObjectOfType<Pathfinder>();

        if (showTrueTile)
        {
            newTileMarker = Instantiate(tileMarker, TileManager.FindTile(transform.position), Quaternion.identity);
        }

        currentTile = TileManager.FindTile(transform.position);
        moving = false;
        path = new List<Vector2>();

        yield return null;

        player.trueTile = currentTile;
        walkHereAction.clientAction0 += ClientClick;
        walkHereAction.serverAction0 += ServerClick;
        walkHereAction.menuTexts[0] = "walk here";
        walkHereAction.staticPlayerActions[0] = true;
        walkHereAction.cancelLevels[0] = 1;
        walkHereAction.inGame = true;
        walkHereAction.redClick = false;
        pathFinder.debugEnabled = debugEnabled;
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
        ClientClick(MouseManager.mouseCoordinate);
    }
    public void ClientClick(Vector2 coordinate)
    {
        Minimap.PlaceFlag(coordinate);
        coordinate = TileManager.FindTile(coordinate);
        destinationTile = coordinate;
        StartCoroutine(ServerClickedTile(destinationTile));
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        if (showClickedTile)
        {
            newClickedTile = Instantiate(clickedTile, destinationTile, Quaternion.identity);
        }
    }

    IEnumerator ServerClickedTile(Vector2 position)
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        serverDestinationTile = position;
    }

    public void ExternalMovement(Vector2 coordinate)
    {
        serverDestinationTile = TileManager.FindTile(coordinate);
        ServerClick();
    }
    void ServerClick()
    {
        Minimap.PlaceFlag(serverDestinationTile);
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        if (showClickedTile)
        {
            newClickedTile = Instantiate(clickedTile, serverDestinationTile, Quaternion.identity);
        }


        path = pathFinder.FindAStarPath(currentTile, serverDestinationTile);


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
                newTileMarker.position = currentTile;
                player.playerPath.Add(currentTile);
                player.trueTile = currentTile;
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
