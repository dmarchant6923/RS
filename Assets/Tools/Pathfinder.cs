using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pathfinder : MonoBehaviour
{
    public bool debugEnabled = false;
    public GameObject AStarDebug;
    GameObject newAStarDebug;

    Vector2 startTile;
    bool startTileFound = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && debugEnabled)
        {
            if (startTileFound == false)
            {
                startTile = TileManager.mouseCoordinate;
                startTileFound = true;
            }
            else
            {
                List<Vector2> path = FindAStarPath(startTile, TileManager.mouseCoordinate);
                for (int i = 0; i < path.Count; i++)
                {
                    newAStarDebug = Instantiate(AStarDebug, path[i], Quaternion.identity);
                    newAStarDebug.GetComponent<SpriteRenderer>().color = Color.blue;
                    newAStarDebug.GetComponentInChildren<TMP_Text>().text = i.ToString();
                    Destroy(newAStarDebug.transform.GetChild(1).gameObject);
                    newAStarDebug.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                startTileFound = false;
            }
        }
    }

    public List<Vector2> FindAStarPath(Vector2 startTile, Vector2 endTile)
    {
        if (debugEnabled)
        {
            foreach (TMP_Text item in FindObjectsOfType<TMP_Text>())
            {
                Destroy(item.transform.root.gameObject);
            }
        }


        List<Vector2> closedTiles = new List<Vector2>();
        List<Vector2> searchedTiles = new List<Vector2>();
        void ExamineTilesForObstacles(List<Vector2> tileList)
        {
            List<Vector2> goodTiles = new List<Vector2>();
            List<Vector2> goodAdjacentTiles = new List<Vector2>();

            foreach(Vector2 tile in tileList)
            {
                for (int i = 0; i < 8; i++)
                {
                    int x = 0; int y = 0;
                    if (i == 1 || i == 2 | i == 3)
                    {
                        x = 1;
                    }
                    if (i == 5 || i == 6 | i == 7)
                    {
                        x = -1;
                    }
                    if (i == 0 || i == 1 | i == 7)
                    {
                        y = 1;
                    }
                    if (i == 3 || i == 4 | i == 5)
                    {
                        y = -1;
                    }

                    Vector2 newTile = tile + new Vector2(x, y);
                    if (closedTiles.Contains(newTile) || searchedTiles.Contains(newTile))
                    {
                        continue;
                    }
                    if (TileDataManager.GetTileData(newTile).obstacle == false)
                    {
                        goodTiles.Add(newTile);
                        if ((newTile - endTile).x == 0 || (newTile - endTile).y == 0)
                        {
                            goodAdjacentTiles.Add(newTile);
                        }
                    }
                    searchedTiles.Add(newTile);
                }
            }

            if (goodTiles.Count > 0)
            {
                Debug.Log(goodAdjacentTiles.Count);
                bool adjacentTileFound = false;
                float dist = 10000;
                Vector2 bestTile = goodTiles[0];
                Vector2 bestAdjacentTile = goodTiles[0];
                foreach (Vector2 goodTile in goodAdjacentTiles)
                {
                    if ((goodTile - startTile).magnitude < dist)
                    {
                        adjacentTileFound = true;
                        dist = (goodTile - startTile).magnitude;
                        bestAdjacentTile = goodTile;
                    }
                }

                dist = 10000;
                foreach (Vector2 goodTile in goodTiles)
                {
                    if ((goodTile - startTile).magnitude < dist)
                    {
                        dist = (goodTile - startTile).magnitude;
                        bestTile = goodTile;
                    }
                }
                if (adjacentTileFound)
                {
                    if (TileManager.TileDistance(startTile, bestTile) >= TileManager.TileDistance(startTile, bestAdjacentTile) - 1)
                    {
                        endTile = bestAdjacentTile;
                    }
                    else
                    {
                        endTile = bestTile;
                    }
                    return;

                }
                endTile = bestTile;
                return;
            }
            else
            {
                closedTiles.AddRange(searchedTiles);
            }
        }


        if (TileDataManager.GetTileData(endTile).obstacle)
        {
            closedTiles.Add(endTile);
            ExamineTilesForObstacles(closedTiles);
            while (TileDataManager.GetTileData(endTile).obstacle)
            {
                ExamineTilesForObstacles(closedTiles);
            }
        }



        //  [G]    [H]
        //      [F]
        //int[2]: G = distance from start tile
        //int[1]: H = distance from end tile
        //int[0]: F = sum of G and H
        //
        //algorithm: First, make sure the selected tile is a valid one. Cannot be on an obstacle.
        //Find tile with lowest F cost.
        //if multiple with equal lowest F cost, find lowest H cost out of those equal tiles.
        //if equal H and F, arbitrarily pick the furthest SE, prioritizing E/W over N/S.

        Dictionary<Vector2, int[]> availableTiles = new Dictionary<Vector2, int[]>();
        Dictionary<Vector2, int[]> examinedTiles = new Dictionary<Vector2, int[]>();
        Dictionary<Vector2, Vector2> previousTile = new Dictionary<Vector2, Vector2>();

        //CalculateValues returns G, H, and F values for a particular tile given the previous tile.
        int[] CalculateValues(Vector2 inputTile, Vector2 fromTile)
        {
            int G = 0;
            if (availableTiles.ContainsKey(fromTile))
            {
                //G = availableTiles[fromTile][2] + (int) ((inputTile - fromTile).magnitude * 10);
                G = availableTiles[fromTile][2] + 10;
            }

            int H = (int) (TileManager.TileDistance(inputTile, endTile) * 10);
            int F = G + H;

            int[] values = new int[3] { F, H, G };
            return values;
        }

        Vector2 selectedTile = startTile;
        availableTiles.Add(startTile, CalculateValues(selectedTile, selectedTile));
        previousTile.Add(startTile, startTile);


        // looks at the 8 spaces surrounding the selected tile, uses CalculateValue to get their values
        void ExamineNewTiles(Vector2 inputTile)
        {
            List<Vector2> ignoreTiles = new List<Vector2>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 addVector = Vector2.zero;
                if (i == 0) { addVector = Vector2.up; }
                if (i == 1) { addVector = Vector2.right; }
                if (i == 2) { addVector = Vector2.down; }
                if (i == 3) { addVector = Vector2.left; }

                Vector2 thisTile = inputTile + addVector;
                if (TileDataManager.GetTileData(thisTile).obstacle)
                {
                    if (i == 0)
                    {
                        ignoreTiles.Add(thisTile);
                        ignoreTiles.Add(inputTile + new Vector2(-1, 1));
                        ignoreTiles.Add(inputTile + new Vector2(1, 1));
                    }
                    if (i == 1)
                    {
                        ignoreTiles.Add(thisTile);
                        ignoreTiles.Add(inputTile + new Vector2(1, 1));
                        ignoreTiles.Add(inputTile + new Vector2(1, -1));
                    }
                    if (i == 2)
                    {
                        ignoreTiles.Add(thisTile);
                        ignoreTiles.Add(inputTile + new Vector2(-1, -1));
                        ignoreTiles.Add(inputTile + new Vector2(1, -1));
                    }
                    if (i == 3)
                    {
                        ignoreTiles.Add(thisTile);
                        ignoreTiles.Add(inputTile + new Vector2(-1, 1));
                        ignoreTiles.Add(inputTile + new Vector2(-1, -1));
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                int x = 0; int y = 0;
                if (i == 1 || i == 2 | i == 3)
                {
                    x = 1;
                }
                if (i == 5 || i == 6 | i == 7)
                {
                    x = -1;
                }
                if (i == 0 || i == 1 | i == 7)
                {
                    y = 1;
                }
                if (i == 3 || i == 4 | i == 5)
                {
                    y = -1;
                }

                Vector2 thisTile = inputTile + new Vector2(x, y);
                if (examinedTiles.ContainsKey(thisTile) || ignoreTiles.Contains(thisTile) || TileDataManager.GetTileData(thisTile).obstacle)
                {
                    continue;
                }

                int[] tileValues = CalculateValues(thisTile, inputTile);
                if (availableTiles.ContainsKey(thisTile))
                {
                    if (availableTiles[thisTile][2] <= tileValues[2])
                    {
                        continue;
                    }
                    else
                    {
                        availableTiles.Remove(thisTile);
                        previousTile.Remove(thisTile);
                    }
                }

                availableTiles.Add(thisTile, tileValues);
                previousTile.Add(thisTile, inputTile);
            }
        }


        // examines all current available tiles, compares their values to see which one to pick.
        Vector2 SelectNewTile()
        {
            List<Vector2> bestTiles = new List<Vector2>();
            float lowestF = 100000;
            float lowestH = 100000;
            foreach (KeyValuePair<Vector2,int[]> tile in availableTiles)
            {
                if (tile.Key == selectedTile || (examinedTiles.Count > 0 && examinedTiles.ContainsKey(tile.Key)))
                {
                    continue;
                }
                if (tile.Value[0] < lowestF)
                {
                    lowestF = tile.Value[0];
                    bestTiles = new List<Vector2>();
                    bestTiles.Add(tile.Key);
                }
                else if (tile.Value[0] == lowestF)
                {
                    bestTiles.Add(tile.Key);
                }

                if (tile.Value[1] == 0)
                {
                    bestTiles = new List<Vector2>();
                    bestTiles.Add(tile.Key);
                    break;
                }
            }

            if (bestTiles.Count > 1)
            {
                List<Vector2> bestTilesAgain = new List<Vector2>();
                foreach (Vector2 tile in bestTiles)
                {
                    if (availableTiles[tile][1] < lowestH)
                    {
                        lowestH = availableTiles[tile][1];
                        bestTilesAgain = new List<Vector2>();
                        bestTilesAgain.Add(tile);
                    }
                    else if (availableTiles[tile][1] == lowestH)
                    {
                        bestTilesAgain.Add(tile);
                    }
                }
                bestTiles = bestTilesAgain;
            }
            else
            {
                return bestTiles[0];
            }

            if (bestTiles.Count > 1)
            {
                Vector2 bestTile = bestTiles[0];
                float dist = 100000;
                foreach (Vector2 tile in bestTiles)
                {
                    if (Mathf.Abs(endTile.x - selectedTile.x) != Mathf.Abs(endTile.y - selectedTile.y) && 
                        Mathf.Abs(tile.x - selectedTile.x) == Mathf.Abs(tile.y - selectedTile.y))
                    {
                        continue;
                    }
                    if ((tile - endTile).magnitude < dist)
                    {
                        dist = (tile - endTile).magnitude;
                        bestTile = tile;
                    }
                }
                return bestTile;
            }
            else
            {
                return bestTiles[0];
            }
        }

        void SpawnDebugMarkers()
        {
            if (debugEnabled == false)
            {
                return;
            }
            foreach (Vector2 tile in availableTiles.Keys)
            {
                RaycastHit2D[] cc = Physics2D.CircleCastAll(tile, 0.2f, Vector2.zero, 0);
                foreach (RaycastHit2D item in cc)
                {
                    if (item.collider != null)
                    {
                        Destroy(item.collider.gameObject);
                    }
                }
                newAStarDebug = Instantiate(AStarDebug, tile, Quaternion.identity);
                newAStarDebug.GetComponent<SpriteRenderer>().color = Color.green;
                newAStarDebug.GetComponentInChildren<TMP_Text>().text =
                    "G: " + availableTiles[tile][2] + "\nH: " + availableTiles[tile][1] + "\nF: " + availableTiles[tile][0];
                newAStarDebug.transform.GetChild(1).eulerAngles = new Vector3(0, 0, VectorToAngle(previousTile[tile] - tile));
            }
            foreach (Vector2 tile in examinedTiles.Keys)
            {
                if (Physics2D.CircleCast(tile, 0.2f, Vector2.zero, 0).collider != null)
                {
                    Destroy(Physics2D.CircleCast(tile, 0.2f, Vector2.zero, 0).collider.gameObject);
                }
                newAStarDebug = Instantiate(AStarDebug, tile, Quaternion.identity);
                newAStarDebug.GetComponent<SpriteRenderer>().color = Color.red;
                newAStarDebug.GetComponentInChildren<TMP_Text>().text =
                    "G: " + examinedTiles[tile][2] + "\nH: " + examinedTiles[tile][1] + "\nF: " + examinedTiles[tile][0];
                newAStarDebug.transform.GetChild(1).eulerAngles = new Vector3(0, 0, VectorToAngle(previousTile[tile] - tile));
            }
            newAStarDebug = Instantiate(AStarDebug, selectedTile, Quaternion.identity);
            newAStarDebug.GetComponentInChildren<TMP_Text>().text =
                "G: " + availableTiles[selectedTile][2] + "\nH: " + availableTiles[selectedTile][1] + "\nF: " + availableTiles[selectedTile][0];
            newAStarDebug.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Destroy(newAStarDebug.transform.GetChild(1).gameObject);
        }

        bool endLoop = false;
        while (selectedTile != endTile && endLoop == false)
        {
            ExamineNewTiles(selectedTile);
            examinedTiles.Add(selectedTile, availableTiles[selectedTile]);

            SpawnDebugMarkers();

            selectedTile = SelectNewTile();

            //float timer = 0;
            //float window = 0.2f;
            //while (timer < window)
            //{
            //    timer += Time.deltaTime;
            //    yield return null;
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        endLoop = true;
            //        break;
            //    }
            //}
        }

        List<Vector2> finalPath = new List<Vector2>();
        Vector2 path = selectedTile;
        finalPath.Add(path);
        while (path != startTile)
        {
            if (debugEnabled)
            {
                newAStarDebug = Instantiate(AStarDebug, path, Quaternion.identity);
                newAStarDebug.GetComponent<SpriteRenderer>().color = Color.blue;
                newAStarDebug.GetComponentInChildren<TMP_Text>().text =
                    "G: " + availableTiles[path][2] + "\nH: " + availableTiles[path][1] + "\nF: " + availableTiles[path][0];
                newAStarDebug.transform.GetChild(1).eulerAngles = new Vector3(0, 0, VectorToAngle(previousTile[path] - path));
                newAStarDebug.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            path = previousTile[path];
            if (path == startTile)
            {
                continue;
            }
            finalPath.Add(path);
            //yield return new WaitForSeconds(0.1f);
        }
        if (debugEnabled)
        {
            newAStarDebug = Instantiate(AStarDebug, path, Quaternion.identity);
            newAStarDebug.GetComponent<SpriteRenderer>().color = Color.blue;
            newAStarDebug.GetComponentInChildren<TMP_Text>().text =
                "G: " + availableTiles[path][2] + "\nH: " + availableTiles[path][1] + "\nF: " + availableTiles[path][0];
            Destroy(newAStarDebug.transform.GetChild(1).gameObject);
            newAStarDebug.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        List<Vector2> reversePath = new List<Vector2>();
        for (int i = finalPath.Count - 1; i >= 0; i--)
        {
            reversePath.Add(finalPath[i]);
        }



        return reversePath;

    }

    public static Vector2 AngleToVector(float angle)
    {
        Vector2 vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad + Mathf.PI / 2),
                Mathf.Sin(angle * Mathf.Deg2Rad + Mathf.PI / 2)).normalized;
        return vector;
    }

    public static float VectorToAngle(Vector2 vector)
    {
        vector = vector.normalized;
        float angle = -Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        return angle;
    }
}
