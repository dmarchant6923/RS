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
    bool clicked = false;

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

    string walkHereString = "Walk here";

    void Start()
    {
        TickManager.onTick += Move;
        MouseManager.IGClientClick += ClientClick;
        MouseManager.IGServerClick += ServerClick;

        pathFinder = FindObjectOfType<Pathfinder>();
        pathFinder.debugEnabled = debugEnabled;

        if (showTrueTile == false)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        currentTile = transform.position;
        moving = false;
        path = new List<Vector2>();
    }

    private void Update()
    {
        if (MouseOverGame.isOverGame && RightClickMenu.menuOpen == false)
        {
            if (RightClickMenu.menuStrings.Contains(walkHereString) == false)
            {
                RightClickMenu.menuStrings.Add(walkHereString);
            }
        }
        else if (RightClickMenu.menuOpen == false)
        {
            if (RightClickMenu.menuStrings.Contains(walkHereString))
            {
                RightClickMenu.menuStrings.Remove(walkHereString);
            }
        }

        if (RightClickMenu.menuOpen)
        {
            if (RightClickMenu.menuStrings.Contains(walkHereString))
            {
                int i = RightClickMenu.menuStrings.IndexOf(walkHereString);
                if (RightClickMenu.newMenu.transform.GetChild(i + 2).GetComponent<MenuEntryClick>().clickMethod == null)
                {
                    RightClickMenu.newMenu.transform.GetChild(i + 2).GetComponent<MenuEntryClick>().clickMethod = MenuClick;
                    Debug.Log((i + 2) + " " + RightClickMenu.newMenu.transform.GetChild(i + 2).GetComponentInChildren<Text>().text);
                }
            }
        }
    }

    public void MenuClick()
    {
        StartCoroutine(MenuClickCR());
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

        clicked = true;
    }

    IEnumerator MenuClickCR()
    {
        ClientClick();
        yield return new WaitForSeconds(TickManager.simLatency);
        ServerClick();
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
    }
}
