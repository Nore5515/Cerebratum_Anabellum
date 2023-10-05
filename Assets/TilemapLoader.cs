using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLoader : MonoBehaviour
{
    string tilemapStateStr = "";

    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    tilemapStateStr += "x:" + x + " y:" + y + " tile:" + tile.name + "\n";
                    //Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    tilemapStateStr += "x:" + x + " y:" + y + " tile: (null)\n";
                    //Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }
        Debug.Log(tilemapStateStr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
