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
    Tilemap wallTileMap;

    [SerializeField]
    Tilemap buildingTileMap;

    [SerializeField]
    TMP_InputField importField;

    [SerializeField]
    TileBase[] tileBases;

    void UpdateStateStr()
    {
        tilemapStateStr = "";
        tilemapStateStr += "--FLOORS--\n";
        tilemapStateStr += GetStringifiedTilemap(GetComponent<Tilemap>());
        tilemapStateStr += "--WALLS--\n";
        tilemapStateStr += GetStringifiedTilemap(wallTileMap);
        tilemapStateStr += "--OBJECTS--\n";
        tilemapStateStr += GetStringifiedTilemap(buildingTileMap);
    }

    string GetStringifiedTilemap(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        string stateStr = "";

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                //TileBase tile = allTiles[x + y * bounds.size.x];
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    stateStr += "x:" + (x) + " y:" + (y) + " tile:" + tile.name + "\n";
                }
            }
        }
        return stateStr;
    }

    public void CopyToClipboard()
    {
        UpdateStateStr();
        GUIUtility.systemCopyBuffer = tilemapStateStr;
    }

    void ClearAllTileMaps()
    {
        GetComponent<Tilemap>().ClearAllTiles();
        wallTileMap.ClearAllTiles();
        buildingTileMap.ClearAllTiles();
    }

    List<TilePosObject> GetTileObjectsFromString(string inputString)
    {
        List<TilePosObject> tileObjs = new List<TilePosObject>();
        List<TilePosObject> floorObjs;
        List<TilePosObject> wallObjs;

        string[] floorsAndWallsString = inputString.Split("--WALLS--");

        string floorTileStrings = floorsAndWallsString[0];
        string wallTileStrings = floorsAndWallsString[1];

        floorObjs = GenerateFloorTilesFromString(floorTileStrings);
        wallObjs = GenerateWallTilesFromString(wallTileStrings);

        foreach (TilePosObject floorObj in floorObjs)
        {
            tileObjs.Add(floorObj);
        }
        foreach (TilePosObject wallObj in wallObjs)
        {
            tileObjs.Add(wallObj);
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

        DisplayTilePosObjectList(floorObjs);

        return floorObjs;
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

        DisplayTilePosObjectList(wallObjs);

        return wallObjs;
    }

    TilePosObject TurnExportStringIntoObj(string str, string layer)
    {
        Debug.Log(str);

        string xStr = str.Substring(0, str.IndexOf("y"));
        int xInt = GetXIntFromXString(xStr);

        int yStrLength = str.IndexOf("tile") - str.IndexOf("y");
        string yStr = str.Substring(xStr.Length, yStrLength);
        int yInt = GetYIntFromYString(yStr);

        int tileStrLength = str.Length - str.IndexOf("tile");
        string tileStr = str.Substring(xStr.Length + yStr.Length, tileStrLength);
        TileBase tile = GetTileBaseFromString(tileStr);

        TilePosObject newTileObj = new TilePosObject(xInt, yInt, tile, layer);
        Debug.Log(newTileObj.tileBase.name + ", (" + newTileObj.x + "," + newTileObj.y + ")");
        return newTileObj;
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
        if (importField.text == "")
        {
            return;
        }

        List<TilePosObject> generatedTiles = GetTileObjectsFromString(importField.text);

        DisplayTilePosObjectList(generatedTiles);

        //TileBase[] allTiles = GetAllTilesFromTilemap(this.GetComponent<Tilemap>());
        //BoundsInt bounds = this.GetComponent<Tilemap>().cellBounds;

        //List<TilePosObject> nonNullTiles = GenerateTilePosArray(allTiles, bounds);

        //foreach (TilePosObject nonNullTile in nonNullTiles)
        //{
        //    Debug.Log(nonNullTile.tileBase.name + ", (" + nonNullTile.x + "," + nonNullTile.y + ")");
        //    Vector3Int pos = new Vector3Int(nonNullTile.x, nonNullTile.y, 0);
        //    GetComponent<Tilemap>().SetTile(pos, nonNullTile.tileBase)
        //}
        ClearAllTileMaps();

        ImplementNewTiles(generatedTiles);

        importField.text = "";
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
                GetComponent<Tilemap>().SetTile(new Vector3Int(tileObj.x, tileObj.y, 0), tileObj.tileBase);
            }
            Debug.Log("nice");
            Debug.Log(tileObj.tileBase.name + ", (" + tileObj.x + "," + tileObj.y + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
