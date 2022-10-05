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
    string openMenuExamineText;
    string menuText;

    Action examineTileAction;
    MenuEntryClick menuEntry;

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
            menuEntry = null;
            examineText = GetTileData(MouseManager.mouseCoordinate).examineText;

            if (examineText != "")
            {
                menuText = "Examine " + GetTileName(MouseManager.mouseCoordinate);
                examineTileAction.menuTexts[0] = menuText;
                if (RightClickMenu.actions.Contains(examineTileAction) == false)
                {
                    RightClickMenu.actions.Add(examineTileAction);
                }
            }
            else if (RightClickMenu.actions.Contains(examineTileAction))
            {
                RightClickMenu.actions.Remove(examineTileAction);
            }
        }

        if (RightClickMenu.menuOpen)
        {
            if (menuEntry == null)
            {
                foreach (MenuEntryClick entry in RightClickMenu.newMenu.GetComponentsInChildren<MenuEntryClick>())
                {
                    if (entry.action == examineTileAction)
                    {
                        menuEntry = entry;
                        return;
                    }
                }
            }
            if (menuEntry != null && menuEntry.clickMethod == null)
            {
                menuEntry.clickMethod = ExamineTile;
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
