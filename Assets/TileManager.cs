using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static Vector2 mouseCoordinate;
    public static Vector2 clickedTile;

    private void Update()
    {
        if (MouseManager.mouseOnScreen && MouseOverGame.isOverGame)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseCoordinate = FindTile(new Vector2(worldPoint.x, worldPoint.y));
        }
    }

    public static Vector2 FindTile(Vector2 coordinate)
    {
        Vector2 scaledCoordinate = new Vector2(coordinate.x, coordinate.y);
        Vector2 returnCoordinate = new Vector2(Mathf.Round(scaledCoordinate.x), Mathf.Round(scaledCoordinate.y));
        return returnCoordinate;
    }

    public static int TileDistance(Vector2 start, Vector2 end)
    {
        start = FindTile(start);
        end = FindTile(end);

        int distance = (int)Mathf.Max(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
        return distance;
    }
}
