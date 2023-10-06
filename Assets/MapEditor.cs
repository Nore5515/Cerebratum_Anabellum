using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MapEditor : MonoBehaviour
{
    [SerializeField]
    Tilemap tileMap;

    [SerializeField]
    Tilemap wallTileMap;

    [SerializeField]
    TileBase floorTile;

    [SerializeField]
    TileBase emptyTile;

    [SerializeField]
    Text debugText;

    bool storedTileFilled = false;
    TileBase storedTile;
    Vector3Int oldGridPos;

    TileBase paletteTile;

    bool selectingWallMap = false;

    void Start()
    {
        paletteTile = floorTile;
    }

    public void SetPaletteTile(TileBase newTile)
    {
        paletteTile = newTile;
        Debug.Log(paletteTile.name);
        if (paletteTile.name.Contains("Wall"))
        {
            selectingWallMap = true;
        }
        else
        {
            selectingWallMap = false;
        }
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
        if (selectingWallMap)
        {
            wallTileMap.SetTile(oldGridPos, storedTile);
        }
        else
        {
            tileMap.SetTile(oldGridPos, storedTile);
        }
        storedTile = null;
        storedTileFilled = false;
    }

    Vector3Int GetGridPos(Vector2 mousePos)
    {
        Vector3Int gridPos;
        if (wallTileMap)
        {
            gridPos = wallTileMap.WorldToCell(mousePos);
        }
        else
        {
            gridPos = tileMap.WorldToCell(mousePos);
        }
        return gridPos;
    }

    bool IsCoveringPortUI(Vector2 mousePos)
    {
        if (mousePos.x - Camera.main.gameObject.transform.position.x <= -7.25f)
        {
            if (mousePos.y - Camera.main.gameObject.transform.position.y >= 3.75f)
            {
                return true;
            }
        }
        return false;
    }

    bool IsCoveringTilePalette(Vector2 mousePos)
    {
        return mousePos.y - Camera.main.gameObject.transform.position.y <= -3.0f;
    }

    bool IsEmptyPaletteSprite()
    {
        if (paletteTile.name.Contains("Empty"))
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(new Vector2(mousePos.x - Camera.main.gameObject.transform.position.x, mousePos.y - Camera.main.gameObject.transform.position.y));
        if (!IsCoveringTilePalette(mousePos) && !IsCoveringPortUI(mousePos))
        {
            debugText.text = "GOOD TO GO";
            Vector3Int gridPos = GetGridPos(mousePos);

            InitGridPos(gridPos);

            if (gridPos != oldGridPos)
            {
                ReplaceOldTileWithStoredTile();
            }

            //if (tileMap.HasTile(gridPos))
            //{
            if (storedTileFilled == false)
            {
                if (selectingWallMap)
                {
                    storedTile = wallTileMap.GetTile(gridPos);
                }
                else
                {
                    storedTile = tileMap.GetTile(gridPos);
                }

                storedTileFilled = true;
            }

            if (selectingWallMap)
            {
                wallTileMap.SetTile(gridPos, paletteTile);
            }
            else
            {
                tileMap.SetTile(gridPos, paletteTile);
            }

            //if (tileMap.GetTile(gridPos).name == "EmptyTile")
            //{
            //}
            //}

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (selectingWallMap)
                {
                    wallTileMap.SetTile(gridPos, paletteTile);
                    tileMap.SetTile(gridPos, null);
                }
                else
                {
                    tileMap.SetTile(gridPos, paletteTile);
                    wallTileMap.SetTile(gridPos, null);
                }

                if (IsEmptyPaletteSprite())
                {
                    wallTileMap.SetTile(gridPos, paletteTile);
                    tileMap.SetTile(gridPos, paletteTile);
                }
                storedTile = paletteTile;
            }

            oldGridPos = gridPos;
        }
        else
        {
            debugText.text = "XXXXXX";
        }
    }
}
