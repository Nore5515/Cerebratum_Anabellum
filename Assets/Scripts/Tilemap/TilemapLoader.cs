using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;


class TilePosObject
{
    public int x, y;
    public TileBase tileBase;
    public string layer;

    public TilePosObject(int x, int y, TileBase tileBase, string layer)
    {
        this.tileBase = tileBase;
        this.x = x;
        this.y = y;
        this.layer = layer;
    }
}

public class TilemapLoader : MonoBehaviour
{
    string tilemapStateStr = "";

    [SerializeField]
    Tilemap floorTileMap;

    [SerializeField]
    Tilemap wallTileMap;

    [SerializeField]
    Tilemap buildingTileMap;

    [SerializeField]
    TMP_InputField importField;

    [SerializeField]
    TileBase[] tileBases;

    [SerializeField]
    bool loadFromSave;

    [SerializeField]
    MapLoaderConstructor constructor;

    [SerializeField]
    NavMeshPlus.Components.NavMeshSurface surface;

    string importJson;

    [SerializeField]
    bool mapEditorLoader = false;

    private void Start()
    {
        if (loadFromSave)
        {
            ImportState();
            GameObject obj = GameObject.FindGameObjectWithTag("spawner_loader");
            SpawnerLoader loader = obj.GetComponent<SpawnerLoader>();
            loader.LateStart();
            //GameObject.FindGameObjectWithTag("spawner_loader").GetComponent<SpawnerLoader>().LateStart();
            StartCoroutine("DelayedBake");
        }
    }

    IEnumerator DelayedBake()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Baking!");
        RebuildNavMesh();
    }

    void UpdateStateStr()
    {
        tilemapStateStr = "";
        tilemapStateStr += "--FLOORS--\n";
        tilemapStateStr += GetStringifiedTilemap(floorTileMap);
        tilemapStateStr += "--WALLS--\n";
        tilemapStateStr += GetStringifiedTilemap(wallTileMap);
        tilemapStateStr += "--OBJECTS--\n";
        tilemapStateStr += GetStringifiedTilemap(buildingTileMap);
    }

    string GetStringifiedTilemap(Tilemap tilemap)
    {
        //tilemap.CompressBounds();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        //tilemap.GetTiles

        string stateStr = "";

        //foreach (Vector3Int position in bounds.allPositionsWithin)
        //{
        //    Debug.Log(position);
        //    if (tilemap.GetTile(position) != null)
        //    {
        //        Vector3 cellPosition = tilemap.GetCellCenterLocal(position);
        //        Debug.Log("x:" + (cellPosition.x) + " y:" + (cellPosition.y) + " tile:" + tilemap.GetTile(position).name + "\n");
        //        stateStr += "x:" + (cellPosition.x) + " y:" + (cellPosition.y) + " tile:" + tilemap.GetTile(position).name + "\n";
        //    }
        //}

        Debug.Log(bounds);

        //tilemap.CompressBounds();
        //for (int x = tilemap.bounds.min.x; x < tilemap.bounds.max.x; x++)
        //{
        //    for (int y = tilemap.bounds.min.y; y < tilemap.bounds.max.y; y++)
        //    {
        //        for (int z = tilemap.bounds.min.z; z < tilemap.bounds.max.z; z++)
        //        {

        //            tilemap.GetTile(new vector3Int(x, y, z));
        //        }
        //    }

        //}

        Debug.Log(bounds.size);
        Vector3Int origin = tilemap.origin;
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                //TileBase tile = allTiles[y + x * bounds.size.y];
                if (tile != null)
                {
                    Debug.Log("x:" + (x + origin.x) + " y:" + (y + origin.y) + " tile:" + tile.name + "\n");
                    stateStr += "x:" + (x + origin.x) + " y:" + (y + origin.y) + " tile:" + tile.name + "\n";
                }
            }
        }


        //for (int x = 0; x < bounds.size.x; x++)
        //{
        //    for (int y = 0; y < bounds.size.y; y++)
        //    {
        //        TileBase tile = allTiles[x + y * bounds.size.x];
        //        //TileBase tile = allTiles[y + x * bounds.size.y];
        //        if (tile != null)
        //        {
        //            Debug.Log("x:" + (x) + " y:" + (y) + " tile:" + tile.name + "\n");
        //            stateStr += "x:" + (x) + " y:" + (y) + " tile:" + tile.name + "\n";
        //        }
        //    }
        //}

        return stateStr;
    }

    public void CopyToClipboard()
    {
        UpdateStateStr();
        GUIUtility.systemCopyBuffer = tilemapStateStr;
    }

    void ClearAllTileMaps()
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
        buildingTileMap.ClearAllTiles();
    }

    List<TilePosObject> GetTileObjectsFromString(string inputString)
    {
        List<TilePosObject> tileObjs = new List<TilePosObject>();
        List<TilePosObject> floorObjs;
        List<TilePosObject> wallObjs;
        List<TilePosObject> objObjs;

        string[] floorsAndWallsString = inputString.Split("--WALLS--");

        string floorTileStrings = floorsAndWallsString[0];

        string wallsAndObjects = floorsAndWallsString[1];

        string[] temp = wallsAndObjects.Split("--OBJECTS--");

        string wallTileStrings = temp[0];
        string objTileStrings = temp[1];

        floorObjs = GenerateFloorTilesFromString(floorTileStrings);
        wallObjs = GenerateWallTilesFromString(wallTileStrings);
        if (!mapEditorLoader)
        {
            ConstructObjectsFromString(objTileStrings);
        }

        foreach (TilePosObject floorObj in floorObjs)
        {
            tileObjs.Add(floorObj);
        }
        foreach (TilePosObject wallObj in wallObjs)
        {
            tileObjs.Add(wallObj);
        }
        if (mapEditorLoader)
        {
            //Debug.Log("Map Editor!");
            objObjs = GenerateObjectTilesFromString(objTileStrings);
            foreach (TilePosObject objObj in objObjs)
            {
                Debug.Log(objObj.layer + ", " + objObj.tileBase.name + ", (" + objObj.x + "," + objObj.y + ")");
                tileObjs.Add(objObj);
            }
        }

        return tileObjs;
    }

    List<TilePosObject> GenerateFloorTilesFromString(string str)
    {
        List<TilePosObject> floorObjs = new List<TilePosObject>();
        string[] floorStringLines = str.Split("\n");

        foreach (string line in floorStringLines)
        {
            if (line != "--FLOORS--" && line != "")
            {
                if (line != "Empty")
                {
                    floorObjs.Add(TurnExportStringIntoObj(line, "floor"));
                }
            }
        }

        //DisplayTilePosObjectList(floorObjs);

        return floorObjs;
    }

    //x:20 y:12 tile:HQBuildingRed
    //x:32 y:38 tile:HQBuildingRed
    void ConstructObjectsFromString(string str)
    {
        string[] objStringLines = str.Split("\n");

        foreach (string line in objStringLines)
        {
            if (line != "--OBJECTS--" && line != "")
            {
                if (line != "Empty")
                {
                    if (line.Contains("HQ"))
                    {
                        Debug.Log(line);
                        Vector2Int coords = ExtractCoordinatesFromString(line);
                        Vector3 worldLoc = floorTileMap.CellToWorld(new Vector3Int(coords.x, coords.y));
                        constructor.PlaceHQAtLocation(worldLoc, GetTeamFromString(line));
                        Debug.Log(coords);
                    }
                    else if (line.Contains("Spawner"))
                    {
                        Debug.Log(line);
                        Vector2Int coords = ExtractCoordinatesFromString(line);
                        Vector3 worldLoc = floorTileMap.CellToWorld(new Vector3Int(coords.x, coords.y));
                        constructor.PlaceSpawnerAtLocation(worldLoc, GetTeamFromString(line));
                        Debug.Log(coords);
                    }
                    else if (line.Contains("Crate"))
                    {
                        Debug.Log(line);
                        Vector2Int coords = ExtractCoordinatesFromString(line);
                        Vector3 worldLoc = floorTileMap.CellToWorld(new Vector3Int(coords.x, coords.y));
                        constructor.PlaceCrateAtLocation(worldLoc);
                        Debug.Log(coords);
                    }
                }
            }
        }
    }

    string GetTeamFromString(string str)
    {
        if (str.Contains("Red"))
        {
            return Constants.RED_TEAM;
        }
        else if (str.Contains("Blue"))
        {
            return Constants.BLUE_TEAM;
        }
        else
        {
            Debug.LogError("INVALID TEAM");
            return null;
        }
    }

    List<TilePosObject> GenerateWallTilesFromString(string str)
    {
        List<TilePosObject> wallObjs = new List<TilePosObject>();
        string[] wallStringLines = str.Split("\n");

        foreach (string line in wallStringLines)
        {
            if (line != "--WALLS--" && line != "" && line != "--OBJECTS--")
            {
                if (line != "Empty")
                {
                    wallObjs.Add(TurnExportStringIntoObj(line, "wall"));
                }
            }
        }

        //DisplayTilePosObjectList(wallObjs);

        return wallObjs;
    }

    List<TilePosObject> GenerateObjectTilesFromString(string str)
    {
        List<TilePosObject> objObjs = new List<TilePosObject>();
        string[] objStringLines = str.Split("\n");

        foreach (string line in objStringLines)
        {
            if (line != "--WALLS--" && line != "" && line != "--OBJECTS--")
            {
                if (line != "Empty")
                {
                    objObjs.Add(TurnExportStringIntoObj(line, "obj"));
                }
            }
        }

        return objObjs;
    }

    TilePosObject TurnExportStringIntoObj(string str, string layer)
    {
        Vector2Int coords = ExtractCoordinatesFromString(str);
        TileBase tile = GetTileBaseFromString(ExtractTileTypeFromString(str));

        TilePosObject newTileObj = new TilePosObject(coords.x, coords.y, tile, layer);
        //Debug.Log(newTileObj.tileBase.name + ", (" + newTileObj.x + "," + newTileObj.y + ")");
        return newTileObj;
    }

    Vector2Int ExtractCoordinatesFromString(string str)
    {
        string xStr = str.Substring(0, str.IndexOf("y"));
        int xInt = GetXIntFromXString(xStr);

        int yStrLength = str.IndexOf("tile") - str.IndexOf("y");
        string yStr = str.Substring(xStr.Length, yStrLength);
        int yInt = GetYIntFromYString(yStr);

        Vector2Int coords = new Vector2Int(xInt, yInt);
        return coords;
    }

    string ExtractTileTypeFromString(string str)
    {
        int tileStrLength = str.Length - str.IndexOf("tile");
        string tileStr = str.Substring(str.IndexOf("tile"), tileStrLength);
        return tileStr;
    }

    TileBase GetTileBaseFromString(string tileStr)
    {
        string cleanedUpStr = tileStr.Substring(5);
        foreach (var tile in tileBases)
        {
            if (tile.name == cleanedUpStr)
            {
                return tile;
            }
        }

        return null;
    }

    bool DoesTilePaletteContainTileByStr(string tileStr)
    {
        string cleanedUpStr = tileStr.Substring(5);
        foreach (var tile in tileBases)
        {
            if (tile.name == cleanedUpStr)
            {
                return true;
            }
        }

        return false;
    }

    int GetYIntFromYString(string yStr)
    {
        string ySubstr = yStr.Substring(2, yStr.Length - 3);
        int yPosInt = int.Parse(ySubstr);
        return yPosInt;
    }

    int GetXIntFromXString(string xStr)
    {
        string xSubstr = xStr.Substring(2, xStr.Length - 3);
        int xPosInt = int.Parse(xSubstr);
        return xPosInt;
    }

    void DisplayTilePosObjectList(List<TilePosObject> objList)
    {
        foreach (TilePosObject tile in objList)
        {
            Debug.Log(tile.tileBase.name + ", (" + tile.x + "," + tile.y + ")");
        }
    }

    public void ImportState()
    {
        Debug.Log("====IMPORT====");
        if (loadFromSave)
        {
            importJson = MapJson.Instance.mapJson;
        }
        else
        {
            importJson = importField.text;
        }

        if (importJson == "")
        {
            return;
        }

        List<TilePosObject> generatedTiles = GetTileObjectsFromString(importJson);

        //DisplayTilePosObjectList(generatedTiles);

        ClearAllTileMaps();

        ImplementNewTiles(generatedTiles);

        if (importField != null)
        {
            importField.text = "";
        }

        RebuildNavMesh();

    }

    public void RebuildNavMesh()
    {
        Debug.Log("Building nav mesh!");
        if (surface != null)
        {
            surface.BuildNavMeshAsync();
        }
    }

    void ImplementNewTiles(List<TilePosObject> newTiles)
    {
        foreach (TilePosObject tileObj in newTiles)
        {
            if (tileObj.layer == "wall")
            {
                wallTileMap.SetTile(new Vector3Int(tileObj.x, tileObj.y, 0), tileObj.tileBase);
            }
            else if (tileObj.layer == "floor")
            {
                floorTileMap.SetTile(new Vector3Int(tileObj.x, tileObj.y, 0), tileObj.tileBase);
            }
            else if (tileObj.layer == "obj")
            {
                buildingTileMap.SetTile(new Vector3Int(tileObj.x, tileObj.y, 0), tileObj.tileBase);
            }
        }
    }
}
