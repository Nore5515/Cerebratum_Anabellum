using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    [SerializeField]
    Sprite tileSprite;

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
        Debug.Log(tileSprite.name);
    }

    // Update is called once per frame
    void Update()
    {
    }

}
