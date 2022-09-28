using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueTile : MonoBehaviour
{
    public GameObject clickedTile;
    GameObject newClickedTile;
    public GameObject debugTile;
    GameObject newDebugTile;
    List<GameObject> debugTiles = new List<GameObject>();
    bool clicked = false;

    public Player player;

    public Vector2 currentTile;
    public Vector2 destinationTile;
    List<Vector2> path;
    public bool moving;
    [HideInInspector] public bool oddTilesInPath = false;

    [HideInInspector] public bool debugEnabled = false;

    void Start()
    {
        TickManager.onTick += Move;
        MouseManager.InGameMouseDown += Click;

        currentTile = transform.position;
        moving = false;
        path = new List<Vector2>();
    }

    void Click()
    {
        if (newClickedTile != null)
        {
            Destroy(newClickedTile);
        }
        destinationTile = TickManager.mouseCoordinate;
        newClickedTile = Instantiate(clickedTile, destinationTile, Quaternion.identity);
        path = new List<Vector2>();
        FindPath(currentTile, destinationTile);
        clicked = true;
    }

    void FindPath(Vector2 startTile, Vector2 endTile)
    {
        Vector2 delta = endTile - startTile;
        Vector2 frontTile = startTile;

        while (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            frontTile = new Vector2(frontTile.x + Mathf.Sign(delta.x), frontTile.y);
            path.Add(frontTile);
            delta = endTile - frontTile;
        }
        while (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            frontTile = new Vector2(frontTile.x, frontTile.y + Mathf.Sign(delta.y));
            path.Add(frontTile);
            delta = endTile - frontTile;
        }
        while (delta.magnitude > 0)
        {
            frontTile = new Vector2(frontTile.x + Mathf.Sign(delta.x), frontTile.y + Mathf.Sign(delta.y));
            path.Add(frontTile);
            delta = endTile - frontTile;
        }
    }

    //updates every tick
    void Move()
    {
        if (clicked)
        {
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
            clicked = false;
        }


        if (path.Count > 0)
        {
            int i = 1;
            if (player.runEnabled) { i = 2; }

            while (i > 0)
            {
                currentTile = path[0];
                transform.position = currentTile;
                player.playerPath.Add(currentTile);
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    moving = false;
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
    }
}
