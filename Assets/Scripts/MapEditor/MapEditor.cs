using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

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
    Tilemap ghostTiles;

    [SerializeField]
    Tilemap buildingMap;

    [SerializeField]
    TileBase floorTile;

    [SerializeField]
    TileBase emptyTile;

    [SerializeField]
    Text debugText;

    [SerializeField]
    Toggle symToggle;

    [SerializeField]
    TileBase eraserTile;

    [SerializeField]
    SettingOptionsLoader optionLoader;

    [SerializeField]
    GameObject settingsMenu;

    [SerializeField]
    GameObject selectionArrow;

    int paintSize = 1;


    bool storedTileFilled = false;
    TileBase storedTile;
    Vector3Int oldGridPos;

    TileBase paletteTile;

    bool selectingWallMap = false;

    bool rightClickHeld = false;

    public bool settingsMode = false;

    TilePlacementAction lastTilesPlaced = new TilePlacementAction();
    List<TilePlacementAction> stashedTileActions = new List<TilePlacementAction>();
    //TilePosObject lastTilePlaced = null;

    void Start()
    {
        paletteTile = floorTile;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PaintTileOnTilemaps(oldGridPos, null, new List<Tilemap> { ghostTiles });
            paintSize = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PaintTileOnTilemaps(oldGridPos, null, new List<Tilemap> { ghostTiles });
            paintSize = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PaintTileOnTilemaps(oldGridPos, null, new List<Tilemap> { ghostTiles });
            paintSize = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            paintSize = 4;
        }

        if (true)
        {
            Vector3Int gridPos = GetGridPos(mousePos);
            string validPos = "";
            validPos += "X: " + mousePos.x;
            validPos += " // Y: " + mousePos.y;
            validPos += "\n\nX: " + gridPos.x;
            validPos += " // Y: " + gridPos.y;
            debugText.text = validPos;

            InitGridPos(gridPos);

            if (gridPos != oldGridPos)
            {
                ReplaceOldTileWithStoredTile();
                PaintTileOnTilemaps(oldGridPos, null, new List<Tilemap> { ghostTiles });
            }

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

            TileBase ghostTile = null;
            if (!settingsMode)
            {
                ghostTile = paletteTile;
                if (IsEmptyPaletteSprite() || rightClickHeld)
                {
                    ghostTile = eraserTile;
                }
                PaintTileOnTilemaps(gridPos, ghostTile, new List<Tilemap> { ghostTiles });
            }

            if (!IsPointerOverUIElement())
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (!settingsMode)
                    {
                        TryDraw(gridPos);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (settingsMode)
                    {
                        SettingsClick(gridPos);
                    }
                    else
                    {
                        DrawTile(gridPos);
                        storedTile = paletteTile;
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (!settingsMode)
                {
                    TryDrawEmpty(gridPos);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                rightClickHeld = true;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                rightClickHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                HandleUndoPress(stashedTileActions);
            }
            oldGridPos = gridPos;
        }
    }

    void SettingsClick(Vector3Int gridPos)
    {
        Debug.Log(buildingMap.GetTile(gridPos));
        if (buildingMap.GetTile(gridPos) != null)
        {
            selectionArrow.SetActive(true);
            Vector3 arrowPos = buildingMap.CellToWorld(gridPos);
            arrowPos.x = arrowPos.x - 0.25f;
            arrowPos.y = arrowPos.y + 1.25f;
            selectionArrow.transform.position = arrowPos;
        }
    }

    public void SetSettingsMode(bool newSettingsMode)
    {
        settingsMode = newSettingsMode;
        settingsMenu.SetActive(settingsMode);
        selectionArrow.SetActive(settingsMode);
    }

    public void SetPaletteTile(TileBase newTile)
    {
        SetSettingsMode(false);
        paletteTile = newTile;
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

    void ReplaceOldTileWithStoredTile()
    {
        ghostTiles.SetTile(oldGridPos, null);

        ghostTiles.SetTile(GetSymPoint(oldGridPos), null);

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

    bool IsEmptyPaletteSprite()
    {
        return IsEmptyTile(paletteTile);
    }

    bool IsEmptyTile(TileBase tile)
    {
        if (tile.name.Contains("Empty") || tile.name.Contains("Eraser"))
        {
            return true;
        }
        return false;
    }

    bool IsBuildingPaletteSprite()
    {
        if (paletteTile.name.Contains("Building"))
        {
            return true;
        }
        return false;
    }

    Vector3Int GetScreenGridCenter()
    {
        Vector2 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        Vector3Int actualCenter = new Vector3Int(0, 0, 0);

        Vector3Int gridPos = actualCenter;

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
            //UndoLastTileAction(stashedActions[stashedActions.Count - 1]);
            stashedActions.RemoveAt(stashedActions.Count - 1);
        }
        else
        {
            return;
        }
    }

    //class TilePosObject
    //{
    //    public int x, y, paintsize;
    //    public TileBase tileBase;
    //    public string layer;

    //    public TilePosObject(int x, int y, TileBase tileBase, string layer)
    //    {
    //        this.tileBase = tileBase;
    //        this.x = x;
    //        this.y = y;
    //        this.layer = layer;
    //    }
    //}


    //void UndoLastTileAction(TilePlacementAction tilePlacementAction)
    //{
    //    foreach (TilePosObject obj in tilePlacementAction.tilePosObjects)
    //    {
    //        if (IsEmptyTile(obj.tileBase))
    //        {

    //        }
    //        if (tileObj.layer == "wall")
    //            {
    //                wallTileMap.SetTile(new Vector3Int(obj.x, obj.y, 0), null);
    //            }
    //            else
    //            {
    //                tileMap.SetTile(new Vector3Int(tileobjObj.x, obj.y, 0), null);
    //            }
    //    }
    //}

    void TryDraw(Vector3Int gridPos)
    {
        if (oldGridPos == gridPos) return;
        DrawTile(gridPos);
        storedTile = paletteTile;
    }

    void PaintTileOnTilemaps(Vector3Int gridPos, TileBase tile, List<Tilemap> tilemaps)
    {
        for (int x = 0; x < paintSize; x++)
        {
            for (int y = 0; y < paintSize; y++)
            {
                Vector3Int pos = new Vector3Int(gridPos.x + x, gridPos.y + y, gridPos.z);
                foreach (Tilemap tilemap in tilemaps)
                {
                    tilemap.SetTile(pos, tile);
                    if (symToggle.isOn)
                    {
                        tilemap.SetTile(GetSymPoint(pos), tile);
                    }
                }
            }
        }
    }

    void DrawTile(Vector3Int gridPos)
    {
        if (IsEmptyPaletteSprite())
        {
            PaintTileOnTilemaps(gridPos, null, new List<Tilemap> { wallTileMap, tileMap, buildingMap });
        }
        else if (IsBuildingPaletteSprite())
        {
            PaintTileOnTilemaps(gridPos, paletteTile, new List<Tilemap> { buildingMap });
        }
        else
        {
            if (selectingWallMap)
            {
                PaintTileOnTilemaps(gridPos, paletteTile, new List<Tilemap> { wallTileMap });
                PaintTileOnTilemaps(gridPos, null, new List<Tilemap> { tileMap });

                // TODO: Move tilepos objects into the PaintTileOnTilemaps function
                TilePlacementAction tileAction = new TilePlacementAction();
                tileAction.tilePosObjects.Add(new TilePosObject(gridPos.x, gridPos.y, paintSize, paletteTile, "wall"));
                stashedTileActions.Add(tileAction);
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    tileAction.tilePosObjects.Add(new TilePosObject(symGridPoint.x, symGridPoint.y, paintSize, paletteTile, "wall"));
                }
            }
            else
            {
                PaintTileOnTilemaps(gridPos, paletteTile, new List<Tilemap> { tileMap });
                PaintTileOnTilemaps(gridPos, null, new List<Tilemap> { wallTileMap });


                TilePlacementAction tileAction = new TilePlacementAction();
                tileAction.tilePosObjects.Add(new TilePosObject(gridPos.x, gridPos.y, paintSize, paletteTile, "floor"));
                stashedTileActions.Add(tileAction);
                if (symToggle.isOn)
                {
                    Vector3Int symGridPoint = GetSymPoint(gridPos);
                    tileAction.tilePosObjects.Add(new TilePosObject(symGridPoint.x, symGridPoint.y, paintSize, paletteTile, "floor"));
                }
            }
        }
    }

    void TryDrawEmpty(Vector3Int gridPos)
    {
        if (oldGridPos == gridPos) return;
        PaintTileOnTilemaps(gridPos, null, new List<Tilemap> { wallTileMap, tileMap, buildingMap });
    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}


