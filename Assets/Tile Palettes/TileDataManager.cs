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
}
