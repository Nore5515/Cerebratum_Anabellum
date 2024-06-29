using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileButton : MonoBehaviour
{
    [SerializeField]
    Sprite tileSprite;

    [SerializeField]
    TileBase tile;

    [SerializeField]
    bool isGearbox;

    MapEditor mapEditor;


    GameObject GetPlayerMapEditor()
    {
        return GameObject.FindGameObjectsWithTag("player_map_editor")[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = tileSprite;
        this.GetComponent<Button>().onClick.AddListener(() => CustomClick());
        mapEditor = GetPlayerMapEditor().GetComponent<MapEditor>();
    }

    void CustomClick()
    {
        if (isGearbox)
        {
            mapEditor.settingsMode = !mapEditor.settingsMode;
        }
        else
        {
            mapEditor.SetPaletteTile(tile);
        }
    }

}
