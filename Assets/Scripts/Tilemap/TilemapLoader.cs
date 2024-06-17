using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System;
using System.IO;

class TilePosObject
{
    public int x, y, paintSize;
    public TileBase tileBase;
    public string layer;

    public TilePosObject(int x, int y, int paintSize, TileBase tileBase, string layer)
    {
        this.tileBase = tileBase;
        this.x = x;
        this.y = y;
        this.paintSize = paintSize;
        this.layer = layer;
    }
}

[Serializable]
public class SaveJSON
{
    //public string saveFile;
    public List<TileSaveObj> floorTilemap;
    public List<TileSaveObj> wallTilemap;
    public List<TileSaveObj> objTilemap;

    public SaveJSON()
    {
        floorTilemap = new List<TileSaveObj>();
        wallTilemap = new List<TileSaveObj>();
        objTilemap = new List<TileSaveObj>();
    }
}

[Serializable]
public struct TileSaveObj
{
    public int x, y;
    public string name;

    public TileSaveObj(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        this.name = name;
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
    string levelTitle;

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

    public void CopyToClipboard()
    {
        SaveJSON save = GetSaveJson();
        string json = JsonUtility.ToJson(save);
        Debug.Log(json);

        GUIUtility.systemCopyBuffer = json;
        CreateTextFile(json);

    }

    void CreateTextFile(string data)
    {
        //Path of the file
        string path = Application.dataPath + "/Log.txt";
        Debug.Log(path);
        File.WriteAllText(path, data);
    }

    SaveJSON GetSaveJson()
    {
        SaveJSON save = new SaveJSON();
        save.floorTilemap = GetTilemapSaveData(floorTileMap);
        save.wallTilemap = GetTilemapSaveData(wallTileMap);
        save.objTilemap = GetTilemapSaveData(buildingTileMap);
        return save;
    }

    List<TileSaveObj> GetTilemapSaveData(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        List<TileSaveObj> saveObjs = new List<TileSaveObj>();

        Vector3Int origin = tilemap.origin;
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    //Debug.Log("x:" + (x + origin.x) + " y:" + (y + origin.y) + " tile:" + tile.name + "\n");
                    TileSaveObj obj = new TileSaveObj(x + origin.x, y + origin.y, tile.name);
                    saveObjs.Add(obj);
                }
            }
        }

        return saveObjs;
    }

    void ClearAllTileMaps()
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
        buildingTileMap.ClearAllTiles();
    }

    List<TilePosObject> GetTileObjectsFromJSON(string inputJSON)
    {
        List<TilePosObject> tileObjs = new List<TilePosObject>();
        List<TilePosObject> floorObjs;
        List<TilePosObject> wallObjs;
        List<TilePosObject> objObjs;


        SaveJSON loadedJSON = JsonUtility.FromJson<SaveJSON>(inputJSON);

        // TODO: Constantify the layer names
        floorObjs = GenerateTilesFromSave(loadedJSON.floorTilemap, "floor");
        wallObjs = GenerateTilesFromSave(loadedJSON.wallTilemap, "wall");
        objObjs = GenerateTilesFromSave(loadedJSON.objTilemap, "obj");

        if (!mapEditorLoader)
        {
            //ConstructObjectsFromString(objTileStrings);
            ConstructObjectsFromJSON(loadedJSON.objTilemap);
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
            foreach (TilePosObject objObj in objObjs)
            {
                Debug.Log(objObj.layer + ", " + objObj.tileBase.name + ", (" + objObj.x + "," + objObj.y + ")");
                tileObjs.Add(objObj);
            }
        }

        return tileObjs;
    }

    //List<TilePosObject> GetTileObjectsFromString(string inputString)
    //{
    //    List<TilePosObject> tileObjs = new List<TilePosObject>();
    //    List<TilePosObject> floorObjs;
    //    List<TilePosObject> wallObjs;
    //    List<TilePosObject> objObjs;

    //    string[] floorsAndWallsString = inputString.Split("--WALLS--");

    //    string floorTileStrings = floorsAndWallsString[0];

    //    string wallsAndObjects = floorsAndWallsString[1];

    //    string[] temp = wallsAndObjects.Split("--OBJECTS--");

    //    string wallTileStrings = temp[0];
    //    string objTileStrings = temp[1];

    //    floorObjs = GenerateFloorTilesFromString(floorTileStrings);
    //    wallObjs = GenerateWallTilesFromString(wallTileStrings);
    //    if (!mapEditorLoader)
    //    {
    //        ConstructObjectsFromString(objTileStrings);
    //    }

    //    foreach (TilePosObject floorObj in floorObjs)
    //    {
    //        tileObjs.Add(floorObj);
    //    }
    //    foreach (TilePosObject wallObj in wallObjs)
    //    {
    //        tileObjs.Add(wallObj);
    //    }
    //    if (mapEditorLoader)
    //    {
    //        //Debug.Log("Map Editor!");
    //        objObjs = GenerateObjectTilesFromString(objTileStrings);
    //        foreach (TilePosObject objObj in objObjs)
    //        {
    //            Debug.Log(objObj.layer + ", " + objObj.tileBase.name + ", (" + objObj.x + "," + objObj.y + ")");
    //            tileObjs.Add(objObj);
    //        }
    //    }

    //    return tileObjs;
    //}


    List<TilePosObject> GenerateTilesFromSave(List<TileSaveObj> savedObjs, string layer)
    {
        List<TilePosObject> objs = new List<TilePosObject>();

        foreach (TileSaveObj obj in savedObjs)
        {
            objs.Add(TurnSaveObjIntoObj(obj, layer));
        }

        return objs;
    }


    //List<TilePosObject> GenerateFloorTilesFromSave(List<TileSaveObj> savedObjs, string layer)
    //{
    //    List<TilePosObject> floorObjs = new List<TilePosObject>();

    //    foreach (TileSaveObj obj in savedObjs)
    //    {
    //        floorObjs.Add(TurnSaveObjIntoObj(obj, layer));
    //    }

    //    return floorObjs;
    //}

    //List<TilePosObject> GenerateFloorTilesFromString(string str)
    //{
    //    List<TilePosObject> floorObjs = new List<TilePosObject>();
    //    string[] floorStringLines = str.Split("\n");

    //    foreach (string line in floorStringLines)
    //    {
    //        if (line != "--FLOORS--" && line != "")
    //        {
    //            if (line != "Empty")
    //            {
    //                floorObjs.Add(TurnExportStringIntoObj(line, "floor"));
    //            }
    //        }
    //    }
    //    return floorObjs;
    //}

    //List<TilePosObject> GenerateWallTilesFromString(string str)
    //{
    //    List<TilePosObject> wallObjs = new List<TilePosObject>();
    //    string[] wallStringLines = str.Split("\n");

    //    foreach (string line in wallStringLines)
    //    {
    //        if (line != "--WALLS--" && line != "" && line != "--OBJECTS--")
    //        {
    //            if (line != "Empty")
    //            {
    //                wallObjs.Add(TurnExportStringIntoObj(line, "wall"));
    //            }
    //        }
    //    }
    //    return wallObjs;
    //}

    //List<TilePosObject> GenerateObjectTilesFromString(string str)
    //{
    //    List<TilePosObject> objObjs = new List<TilePosObject>();
    //    string[] objStringLines = str.Split("\n");

    //    foreach (string line in objStringLines)
    //    {
    //        if (line != "--WALLS--" && line != "" && line != "--OBJECTS--")
    //        {
    //            if (line != "Empty")
    //            {
    //                objObjs.Add(TurnExportStringIntoObj(line, "obj"));
    //            }
    //        }
    //    }

    //    return objObjs;
    //}

    void ConstructObjectsFromJSON(List<TileSaveObj> objs)
    {
        foreach (TileSaveObj obj in objs)
        {
            Vector2Int coords = new Vector2Int(obj.x, obj.y);
            Vector3 worldLoc = floorTileMap.CellToWorld(new Vector3Int(coords.x, coords.y));
            Debug.Log(obj);
            if (obj.name.Contains("HQ"))
            {
                constructor.PlaceHQAtLocation(worldLoc, GetTeamFromString(obj.name));
            }
            else if (obj.name.Contains("Spawner"))
            {
                constructor.PlaceSpawnerAtLocation(worldLoc, GetTeamFromString(obj.name));
            }
            else if (obj.name.Contains("Crate"))
            {
                constructor.PlaceCrateAtLocation(worldLoc);
            }
            else
            {
                Debug.LogError("UNKNWON OBJECT: " + obj);
            }
        }
    }

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

    TilePosObject TurnSaveObjIntoObj(TileSaveObj obj, string layer)
    {
        Vector2Int coords = new Vector2Int(obj.x, obj.y);
        TileBase tile = GetTileBaseFromString(obj.name);
        TilePosObject newTileObj = new TilePosObject(coords.x, coords.y, 1, tile, layer);
        return newTileObj;
    }

    TilePosObject TurnExportStringIntoObj(string str, string layer)
    {
        Vector2Int coords = ExtractCoordinatesFromString(str);
        TileBase tile = GetTileBaseFromString(ExtractTileTypeFromString(str.Substring(5)));

        TilePosObject newTileObj = new TilePosObject(coords.x, coords.y, 1, tile, layer);
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
        foreach (var tile in tileBases)
        {
            if (tile.name == tileStr)
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
            if (MapJson.Instance.jsons.ContainsKey(levelTitle))
            {
                importJson = MapJson.Instance.jsons[levelTitle];
            }
            else
            {
                importJson = MapJson.Instance.mapJson;
            }
        }
        else
        {
            importJson = importField.text;
        }

        if (importJson == "")
        {
            return;
        }

        //List<TilePosObject> generatedTiles = GetTileObjectsFromString(importJson);
        List<TilePosObject> generatedTiles = GetTileObjectsFromJSON(importJson);

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
