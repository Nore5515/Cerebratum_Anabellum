using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class TilemapLoader : MonoBehaviour
{
    string tilemapStateStr = "";

    [SerializeField]
    Tilemap wallTileMap;

    [SerializeField]
    TMP_InputField importField;


    // Start is called before the first frame update
    void Start()
    {
    }

    void UpdateStateStr()
    {
        tilemapStateStr += "--FLOORS--\n";
        tilemapStateStr += GetStringifiedTilemap(GetComponent<Tilemap>());
        tilemapStateStr += "--WALLS--\n";
        tilemapStateStr += GetStringifiedTilemap(wallTileMap);

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
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    stateStr += "x:" + x + " y:" + y + " tile:" + tile.name + "\n";
                    //Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
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

    public void ImportState()
    {
        Debug.Log(importField.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
