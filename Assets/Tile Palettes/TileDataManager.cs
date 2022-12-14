using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileDataManager : MonoBehaviour
{
    [SerializeField] Tilemap map;
    [SerializeField] static Tilemap staticMap;

    [SerializeField] TileData defaultTileData;
    [SerializeField] static TileData staticDefaultTileData;
    [SerializeField] List<TileData> tileDatas;

    public static Dictionary<TileBase, TileData> dataFromTiles;

    string examineText;
    string menuText;

    Action examineTileAction;

    private void Start()
    {
        staticMap = map;
        staticDefaultTileData = defaultTileData;
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        examineTileAction = GetComponent<Action>();
        examineTileAction.orderLevels[8] = -1;
    }

    private void Update()
    {
        if (RightClickMenu.menuOpen == false)
        {
            if (MouseManager.isOverGame)
            {
                examineText = GetTileData(MouseManager.mouseCoordinate).examineText;
            }
            else
            {
                examineText = "";
            }

            if (examineText != "")
            {
                menuText = "Examine <color=cyan>" + GetTileName(MouseManager.mouseCoordinate) + "</color>";
                examineTileAction.menuTexts[8] = menuText;
                examineTileAction.examineText = examineText;
                examineTileAction.objectName = GetTileName(MouseManager.mouseCoordinate);
                if (RightClickMenu.tileActions.Contains(examineTileAction) == false)
                {
                    RightClickMenu.tileActions.Add(examineTileAction);
                }
            }
            else if (RightClickMenu.tileActions.Contains(examineTileAction))
            {
                RightClickMenu.tileActions.Remove(examineTileAction);
            }
        }
    }

    public static TileData GetTileData(Vector2 coordinate)
    {
        Vector3Int gridPosition = staticMap.WorldToCell(coordinate);
        TileBase selectedTile = staticMap.GetTile(gridPosition);

        if (selectedTile != null)
        {
            return dataFromTiles[selectedTile];
        }
        else
        {
            return staticDefaultTileData;
        }
    }

    public static bool ObstacleInArea(Vector2 SWTile, int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 tile = SWTile + Vector2.right * i + Vector2.up * j;
                if (GetTileData(tile).obstacle)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static string GetTileName(Vector2 coordinate)
    {
        Vector3Int gridPosition = staticMap.WorldToCell(coordinate);
        TileBase selectedTile = staticMap.GetTile(gridPosition);

        if (selectedTile != null)
        {
            return selectedTile.name;
        }
        else
        {
            return "";
        }
    }

    private void OnDestroy()
    {
        if (RightClickMenu.tileActions.Contains(examineTileAction))
        {
            RightClickMenu.tileActions.Remove(examineTileAction);
        }
    }
}
