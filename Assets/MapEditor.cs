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

    bool storedTileFilled = false;
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
        storedTileFilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.y >= -9.5f)
        {
            Vector3Int gridPos = tileMap.WorldToCell(mousePos);

            InitGridPos(gridPos);

            if (gridPos != oldGridPos)
            {
                ReplaceOldTileWithStoredTile();
            }

            //if (tileMap.HasTile(gridPos))
            //{
            if (storedTileFilled == false)
            {
                storedTile = tileMap.GetTile(gridPos);
                storedTileFilled = true;
            }
            tileMap.SetTile(gridPos, floorTile);

            //if (tileMap.GetTile(gridPos).name == "EmptyTile")
            //{
            //}
            //}

            if (Input.GetKey(KeyCode.Mouse0))
            {
                tileMap.SetTile(gridPos, floorTile);
                storedTile = floorTile;
            }

            oldGridPos = gridPos;
        }
    }
}
