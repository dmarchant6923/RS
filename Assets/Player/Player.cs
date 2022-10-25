using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public bool runEnabled;
    [System.NonSerialized] public bool forceWalk = false;
    public GameObject trueTileObject;
    GameObject newTrueTileObject;
    [HideInInspector] public TrueTile trueTile;
    public Vector2 playerPosition;
    float moveSpeed = 3f;
    public List<Vector2> playerPath;

    public Vector2 truePlayerTile;

    public float runEnergy = 500;
    [System.NonSerialized] public float weight = 40;

    [HideInInspector] public bool showTrueTile = false;
    [HideInInspector] public bool showClickedTile = false;
    public bool debugEnabled = false;

    void Start()
    {
        Vector2 startTile = TileManager.FindTile(transform.position);
        transform.position = startTile;

        newTrueTileObject = Instantiate(trueTileObject, startTile, Quaternion.identity);
        trueTile = newTrueTileObject.GetComponent<TrueTile>();
        trueTile.player = this;
        trueTile.debugEnabled = debugEnabled;
        trueTile.showTrueTile = showTrueTile;
        trueTile.showClickedTile = showClickedTile;

        runEnabled = false;

        playerPath = new List<Vector2>();

        TickManager.onTick += RunEnergy;
    }

    // Update is called once per frame
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
            if ((playerPosition - trueTile.currentTile).magnitude > 2 || playerPath.Count > 2)
            {
                trueMoveSpeed *= 1.3f;
            }

            transform.position = Vector2.MoveTowards(transform.position, playerPath[0], trueMoveSpeed * Time.deltaTime);

            if ((playerPosition - playerPath[0]).magnitude < 0.01f)
            {
                transform.position = playerPath[0];
                playerPath.RemoveAt(0);
                forceWalk = false;
                if (runEnabled && playerPath.Count > 0 && playerPath[0] == trueTile.destinationTile && trueTile.oddTilesInPath)
                {
                    forceWalk = true;
                }
            }
        }
    }

    //updates on every tick
    void RunEnergy()
    {
        if (trueTile.moving && runEnabled && forceWalk == false)
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
    }
}
