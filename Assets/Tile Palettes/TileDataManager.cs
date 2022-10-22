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
    }

    private void Update()
    {
        if (RightClickMenu.menuOpen == false)
        {
            examineText = GetTileData(MouseManager.mouseCoordinate).examineText;

            if (examineText != "")
            {
                menuText = "Examine " + GetTileName(MouseManager.mouseCoordinate);
                examineTileAction.menuTexts[5] = menuText;
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

    public void ExamineTile()
    {
        Debug.Log(examineText);
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
}
