using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

// Contains a list of tile pos objects (i.e. sym placed both go in here)
class TilePlacementAction
{
    public List<TilePosObject> tilePosObjects;

    public TilePlacementAction()
    {
        tilePosObjects = new List<TilePosObject>();
    }
}

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

    TilePlacementAction lastTilesPlaced = new TilePlacementAction();
    List<TilePlacementAction> stashedTileActions = new List<TilePlacementAction>();
    //TilePosObject lastTilePlaced = null;

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

        Vector3Int actualCenter = new Vector3Int(0, 0, 0);

        gridPos = actualCenter;

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

    void HandleUndoPress(List<TilePlacementAction> stashedActions)
    {
        if (stashedActions.Count > 0)
        {
            UndoLastTileAction(stashedActions[stashedActions.Count - 1]);
            stashedActions.RemoveAt(stashedActions.Count - 1);
        }
        else
        {
            return;
        }
    }

    void UndoLastTileAction(TilePlacementAction tilePlacementAction)
    {
        foreach (var tileObjToUndo in tilePlacementAction.tilePosObjects)
        {
            if (tileObjToUndo.layer == "wall")
            {
                wallTileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
            }
            else
            {
                tileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
            }
        }
    }

    //void UndoTiles(List<TilePosObject> tileObjsToUndo)
    //{
    //    foreach (var tileObjToUndo in tileObjsToUndo)
    //    {
    //        if (tileObjToUndo.layer == "wall")
    //        {
    //            wallTileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
    //        }
    //        else
    //        {
    //            tileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
    //        }
    //    }
    //}


    //void UndoTile(TilePosObject tileObjToUndo)
    //{
    //    if (tileObjToUndo.layer == "wall")
    //    {
    //        wallTileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
    //    }
    //    else
    //    {
    //        tileMap.SetTile(new Vector3Int(tileObjToUndo.x, tileObjToUndo.y, 0), null);
    //    }
    //}

    void DrawTile(Vector3Int gridPos)
    {
        //if (lastTilesPlaced.tilePosObjects.Count != 0)
        //{
        //    foreach (var tilePlacementAction in lastTilesPlaced.tilePosObjects)
        //    {
        //        Debug.Log(tilePlacementAction.x + "," + tilePlacementAction.y + "," + tilePlacementAction.tileBase.name + "," + tilePlacementAction.layer);
        //    }
        //}
        //lastTilesPlaced.tilePosObjects.Clear();
        if (IsEmptyPaletteSprite())
        {
            wallTileMap.SetTile(gridPos, paletteTile);
            tileMap.SetTile(gridPos, paletteTile);
        }
        else
        {
            if (selectingWallMap)
            {
                TilePlacementAction tileAction = new TilePlacementAction();
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    wallTileMap.SetTile(symGridPoint, paletteTile);
                    tileMap.SetTile(symGridPoint, null);
                    tileAction.tilePosObjects.Add(new TilePosObject(symGridPoint.x, symGridPoint.y, paletteTile, "wall"));
                }
                tileAction.tilePosObjects.Add(new TilePosObject(gridPos.x, gridPos.y, paletteTile, "wall"));
                wallTileMap.SetTile(gridPos, paletteTile);
                tileMap.SetTile(gridPos, null);
                stashedTileActions.Add(tileAction);
            }
            else
            {
                TilePlacementAction tileAction = new TilePlacementAction();
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    tileMap.SetTile(symGridPoint, paletteTile);
                    wallTileMap.SetTile(symGridPoint, null);
                    tileAction.tilePosObjects.Add(new TilePosObject(symGridPoint.x, symGridPoint.y, paletteTile, "floor"));
                }
                tileAction.tilePosObjects.Add(new TilePosObject(gridPos.x, gridPos.y, paletteTile, "floor"));
                tileMap.SetTile(gridPos, paletteTile);
                wallTileMap.SetTile(gridPos, null);
                stashedTileActions.Add(tileAction);
            }
        }
    }

    void TryDraw(Vector3Int gridPos)
    {
        if (oldGridPos == gridPos) return;
        DrawTile(gridPos);
        storedTile = paletteTile;
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
                TryDraw(gridPos);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                DrawTile(gridPos);
                storedTile = paletteTile;
            }


            if (Input.GetKeyDown(KeyCode.Z))
            {
                //UndoTile(lastTilePlaced);
                //UndoTiles(lastTilesPlaced);
                //UndoLastTileAction(lastTilesPlaced);
                HandleUndoPress(stashedTileActions);
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


