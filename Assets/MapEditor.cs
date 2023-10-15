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

    [SerializeField]
    Toggle symToggle;

    bool storedTileFilled = false;
    TileBase storedTile;
    Vector3Int oldGridPos;

    TileBase paletteTile;

    bool selectingWallMap = false;

    void Start()
    {
        paletteTile = floorTile;
    }

    void RevertTile()
    {
        wallTileMap.SetTile(oldGridPos, storedTile);
        tileMap.SetTile(oldGridPos, storedTile);
    }

    public void SetPaletteTile(TileBase newTile)
    {
        //RevertTile();
        paletteTile = newTile;
        //Debug.Log(paletteTile.name);
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
            if (mousePos.y - Camera.main.gameObject.transform.position.y >= 3.25f)
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

    Vector3Int GetScreenGridCenter()
    {
        Vector2 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        Vector3Int gridPos = GetGridPos(centerPos);

        return gridPos;
    }

    Vector3Int GetSymPoint(Vector3Int gridPos)
    {
        Vector3Int symGridPos = new Vector3Int(0, 0, gridPos.z);
        Vector3Int centerPos = GetScreenGridCenter();

        int xDistFromCenter = gridPos.x - centerPos.x;
        int yDistFromCenter = gridPos.y - centerPos.y;

        symGridPos.x = centerPos.x - xDistFromCenter;
        symGridPos.y = centerPos.y - yDistFromCenter;

        return symGridPos;
    }

    void DrawTile(Vector3Int gridPos)
    {
        if (IsEmptyPaletteSprite())
        {
            wallTileMap.SetTile(gridPos, paletteTile);
            tileMap.SetTile(gridPos, paletteTile);
        }
        else
        {
            if (selectingWallMap)
            {
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    wallTileMap.SetTile(symGridPoint, paletteTile);
                    tileMap.SetTile(symGridPoint, null);
                }
                wallTileMap.SetTile(gridPos, paletteTile);
                tileMap.SetTile(gridPos, null);
            }
            else
            {
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    tileMap.SetTile(symGridPoint, paletteTile);
                    wallTileMap.SetTile(symGridPoint, null);
                }
                tileMap.SetTile(gridPos, paletteTile);
                wallTileMap.SetTile(gridPos, null);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(new Vector2(mousePos.x - Camera.main.gameObject.transform.position.x, mousePos.y - Camera.main.gameObject.transform.position.y));

        if (!IsCoveringTilePalette(mousePos) && !IsCoveringPortUI(mousePos))
        {

            string validPos = "";
            validPos += "X: " + mousePos.x;
            validPos += " // Y: " + mousePos.y;
            debugText.text = validPos;
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
                DrawTile(gridPos);
                storedTile = paletteTile;
            }

            oldGridPos = gridPos;
        }
        else
        {
            RevertTile();
            debugText.text = "XXXXXX";
        }
    }
}


