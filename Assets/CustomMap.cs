using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--FLOORS--
//x: 46 y: 58 tile: FloorTile
//x:46 y: 59 tile: FloorTile
//x:46 y: 60 tile: FloorTile
//x:46 y: 61 tile: FloorTile
//x:47 y: 57 tile: FloorTile
//x:47 y: 58 tile: FloorTile
//x:47 y: 59 tile: FloorTile
//x:47 y: 60 tile: FloorTile
//x:47 y: 61 tile: FloorTile
//x:47 y: 62 tile: FloorTile
//x:48 y: 56 tile: FloorTile
//x:48 y: 57 tile: FloorTile
//x:48 y: 58 tile: FloorTile
//x:48 y: 59 tile: FloorTile
//x:48 y: 60 tile: FloorTile
//x:48 y: 61 tile: FloorTile
//x:48 y: 62 tile: FloorTile
//x:48 y: 63 tile: FloorTile
//x:49 y: 53 tile: FloorTile
//x:49 y: 54 tile: FloorTile
//x:49 y: 55 tile: FloorTile
//x:49 y: 56 tile: FloorTile
//x:49 y: 57 tile: FloorTile
//x:49 y: 58 tile: FloorTile
//x:49 y: 59 tile: FloorTile
//x:49 y: 60 tile: FloorTile
//x:49 y: 61 tile: FloorTile
//x:49 y: 62 tile: FloorTile
//x:49 y: 63 tile: FloorTile
//x:49 y: 64 tile: FloorTile
//x:50 y: 44 tile: FloorTile
//x:50 y: 45 tile: FloorTile
//x:50 y: 46 tile: FloorTile
//x:50 y: 53 tile: FloorTile
//x:50 y: 54 tile: FloorTile
//x:50 y: 55 tile: FloorTile
//x:50 y: 56 tile: FloorTile
//x:50 y: 57 tile: FloorTile
//x:50 y: 58 tile: FloorTile
//x:50 y: 59 tile: FloorTile
//x:50 y: 60 tile: FloorTile
//x:50 y: 61 tile: FloorTile
//x:50 y: 62 tile: FloorTile
//x:50 y: 63 tile: FloorTile
//x:50 y: 64 tile: FloorTile
//x:51 y: 44 tile: FloorTile
//x:51 y: 45 tile: FloorTile
//x:51 y: 46 tile: FloorTile
//x:51 y: 47 tile: FloorTile
//x:51 y: 48 tile: FloorTile
//x:51 y: 50 tile: FloorTile
//x:51 y: 51 tile: FloorTile
//x:51 y: 52 tile: FloorTile
//x:51 y: 53 tile: FloorTile
//x:51 y: 54 tile: FloorTile
//x:51 y: 55 tile: FloorTile
//x:51 y: 56 tile: FloorTile
//x:51 y: 57 tile: FloorTile
//x:51 y: 58 tile: FloorTile
//x:51 y: 59 tile: FloorTile
//x:51 y: 60 tile: FloorTile
//x:51 y: 61 tile: FloorTile
//x:51 y: 62 tile: FloorTile
//x:51 y: 63 tile: FloorTile
//x:51 y: 64 tile: FloorTile
//x:52 y: 43 tile: FloorTile
//x:52 y: 44 tile: FloorTile
//x:52 y: 45 tile: FloorTile
//x:52 y: 46 tile: FloorTile
//x:52 y: 47 tile: FloorTile
//x:52 y: 48 tile: FloorTile
//x:52 y: 49 tile: FloorTile
//x:52 y: 50 tile: FloorTile
//x:52 y: 51 tile: FloorTile
//x:52 y: 52 tile: FloorTile
//x:52 y: 53 tile: FloorTile
//x:52 y: 54 tile: FloorTile
//x:52 y: 55 tile: FloorTile
//x:52 y: 56 tile: FloorTile
//x:52 y: 58 tile: FloorTile
//x:52 y: 59 tile: FloorTile
//x:52 y: 60 tile: FloorTile
//x:52 y: 61 tile: FloorTile
//x:52 y: 62 tile: FloorTile
//x:52 y: 63 tile: FloorTile
//x:52 y: 64 tile: FloorTile
//x:52 y: 65 tile: FloorTile
//x:53 y: 43 tile: FloorTile
//x:53 y: 44 tile: FloorTile
//x:53 y: 45 tile: FloorTile
//x:53 y: 46 tile: FloorTile
//x:53 y: 47 tile: FloorTile
//x:53 y: 48 tile: FloorTile
//x:53 y: 49 tile: FloorTile
//x:53 y: 50 tile: FloorTile
//x:53 y: 51 tile: FloorTile
//x:53 y: 52 tile: FloorTile
//x:53 y: 53 tile: FloorTile
//x:53 y: 54 tile: FloorTile
//x:53 y: 59 tile: FloorTile
//x:53 y: 60 tile: FloorTile
//x:53 y: 61 tile: FloorTile
//x:53 y: 62 tile: FloorTile
//x:53 y: 63 tile: FloorTile
//x:53 y: 64 tile: FloorTile
//x:53 y: 65 tile: FloorTile
//x:54 y: 43 tile: FloorTile
//x:54 y: 44 tile: FloorTile
//x:54 y: 45 tile: FloorTile
//x:54 y: 46 tile: FloorTile
//x:54 y: 47 tile: FloorTile
//x:54 y: 48 tile: FloorTile
//x:54 y: 49 tile: FloorTile
//x:54 y: 50 tile: FloorTile
//x:54 y: 51 tile: FloorTile
//x:54 y: 52 tile: FloorTile
//x:54 y: 59 tile: FloorTile
//x:54 y: 60 tile: FloorTile
//x:54 y: 61 tile: FloorTile
//x:54 y: 62 tile: FloorTile
//x:54 y: 63 tile: FloorTile
//x:54 y: 64 tile: FloorTile
//x:54 y: 65 tile: FloorTile
//x:55 y: 44 tile: FloorTile
//x:55 y: 45 tile: FloorTile
//x:55 y: 46 tile: FloorTile
//x:55 y: 47 tile: FloorTile
//x:55 y: 48 tile: FloorTile
//x:55 y: 49 tile: FloorTile
//x:55 y: 50 tile: FloorTile
//x:55 y: 51 tile: FloorTile
//x:55 y: 60 tile: FloorTile
//x:55 y: 61 tile: FloorTile
//x:55 y: 62 tile: FloorTile
//x:55 y: 63 tile: FloorTile
//x:55 y: 64 tile: FloorTile
//x:56 y: 44 tile: FloorTile
//x:56 y: 45 tile: FloorTile
//x:56 y: 46 tile: FloorTile
//x:56 y: 47 tile: FloorTile
//x:56 y: 48 tile: FloorTile
//x:56 y: 49 tile: FloorTile
//x:56 y: 50 tile: FloorTile
//x:56 y: 51 tile: FloorTile
//x:56 y: 62 tile: FloorTile
//x:56 y: 63 tile: FloorTile
//x:56 y: 64 tile: FloorTile
//x:57 y: 44 tile: FloorTile
//x:57 y: 45 tile: FloorTile
//x:57 y: 46 tile: FloorTile
//x:57 y: 47 tile: FloorTile
//x:57 y: 48 tile: FloorTile
//x:57 y: 49 tile: FloorTile
//x:57 y: 50 tile: FloorTile
//x:57 y: 51 tile: FloorTile
//x:58 y: 45 tile: FloorTile
//x:58 y: 46 tile: FloorTile
//x:58 y: 47 tile: FloorTile
//x:58 y: 48 tile: FloorTile
//x:58 y: 49 tile: FloorTile
//x:58 y: 50 tile: FloorTile
//x:58 y: 51 tile: FloorTile
//x:58 y: 52 tile: FloorTile
//x:59 y: 46 tile: FloorTile
//x:59 y: 47 tile: FloorTile
//x:59 y: 48 tile: FloorTile
//x:59 y: 49 tile: FloorTile
//x:59 y: 50 tile: FloorTile
//x:59 y: 51 tile: FloorTile
//x:60 y: 47 tile: FloorTile
//x:60 y: 48 tile: FloorTile
//x:60 y: 49 tile: FloorTile
//x:60 y: 50 tile: FloorTile
//--WALLS--
//--OBJECTS--
//x: 52 y: 67 tile: SpawnerBuildingBlue
//x:57 y: 50 tile: HQBuildingRed
//x:57 y: 70 tile: HQBuildingBlue
//x:62 y: 55 tile: SpawnerBuildingRed


public class CustomMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(MapJson.Instance.mapJson);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
