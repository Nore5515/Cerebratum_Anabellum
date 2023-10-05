using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : MonoBehaviour
{
    [SerializeField]
    Tilemap tileMap;

    [SerializeField]
    TileBase floorTile;

    [SerializeField]
    TileBase emptyTile;

    TileBase storedTile;
    Vector3Int oldGridPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void InitGridPos(Vector3Int gridPos)
    {
        if (oldGridPos == null)
        {
            oldGridPos = gridPos;
        }
    }

    void StoreNewTile(TileBase tile)
    {
        storedTile = tile;
    }

    void ReplaceOldTileWithStoredTile()
    {
        tileMap.SetTile(oldGridPos, storedTile);
        storedTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tileMap.WorldToCell(mousePos);

        InitGridPos(gridPos);

        if (gridPos != oldGridPos)
        {
            ReplaceOldTileWithStoredTile();
        }

        if (tileMap.HasTile(gridPos))
        {
            if (storedTile == null)
            {
                storedTile = tileMap.GetTile(gridPos);
            }
            tileMap.SetTile(gridPos, floorTile);

            //if (tileMap.GetTile(gridPos).name == "EmptyTile")
            //{
            //}
        }

        oldGridPos = gridPos;
    }
}
