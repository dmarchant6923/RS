using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public delegate void TickAction();
    public static event TickAction beforeTick;
    public static event TickAction onTick;
    public static event TickAction afterTick;

    public const float maxTickTime = 0.6f;
    public static int tickCount;
    float tickTimer;
    public static float simLatency = 0.1f;

    public static Vector2 mouseCoordinate;
    public static float gridSize = 1;

    public static Vector2 clickedTile;

    private void Awake()
    {
        tickCount = 0;
        tickTimer = 0;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= maxTickTime)
        {
            tickTimer -= maxTickTime;
            tickCount++;
            if (beforeTick != null)
            {
                beforeTick();
            }
            if (onTick != null)
            {
                onTick();
            }
            if (afterTick != null)
            {
                afterTick();
            }
        }

        if (MouseManager.mouseOnScreen)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseCoordinate = FindTile(new Vector2(worldPoint.x, worldPoint.y));
        }
    }

    public static Vector2 FindTile(Vector2 coordinate)
    {
        Vector2 scaledCoordinate = new Vector2(coordinate.x, coordinate.y);
        Vector2 returnCoordinate = new Vector2(Mathf.Round(scaledCoordinate.x) * gridSize, Mathf.Round(scaledCoordinate.y) * gridSize);
        return returnCoordinate;
    }

    public static int TileDistance(Vector2 start, Vector2 end)
    {
        start = FindTile(start);
        end = FindTile(end);

        int distance = (int) Mathf.Max(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
        return distance;

    }
}
