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

    Vector3Int oldGridPos;
    TileBase oldGridTile;

    bool tilePlaced = false;

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

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tileMap.WorldToCell(mousePos);

        InitGridPos(gridPos);

        if (gridPos != oldGridPos)
        {
            tileMap.SetTile(oldGridPos, oldGridTile);
        }

        if (tileMap.HasTile(gridPos))
        {
            Debug.Log(tileMap.GetTile(gridPos).name);
            if (tileMap.GetTile(gridPos).name == "EmptyTile" && !tilePlaced)
            {
                tileMap.SetTile(gridPos, floorTile);
            }
            oldGridTile = tileMap.GetTile(gridPos);
        }

        oldGridPos = gridPos;
    }
}
