using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will handle the creation of new units!
/// It will be passed multiple parameters, and return a unit.
/// (Or just place the unit where it is to be. not sure yet.)

/// This should get off-topic code out from Spawner...
/// ...and allow other scripts to create units! Think of the possibilities!
/// </summary>
public class UnitFactory : MonoBehaviour
{
    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();
    [SerializeField] GameObject scoutPrefab;
    [SerializeField] GameObject infantryPrefab;

    public Material redMat;
    public Material blueMat;

    const float OFFSET_MAX = 3;

    GameObject GetPrefabFromUnitString(string unitTypeString)
    {
        switch (unitTypeString)
        {
            case ("Infantry"):
                return infantryPrefab;
            case ("Scout"):
                return scoutPrefab;
            default:
                Debug.LogError("UNKNOWN UNIT TYPE STRING");
                return null;
        }
    }

    private List<Vector3> OffsetUnitPathPoints(List<Vector3> pathPoints)
    {
        List<Vector3> offsetPoints = new List<Vector3>();
        Vector3 offsetPoint;

        float xOffset = Random.Range(-OFFSET_MAX, OFFSET_MAX);
        float yOffset = Random.Range(-OFFSET_MAX, OFFSET_MAX);

        foreach (Vector3 pathPoint in pathPoints)
        {
            offsetPoint = new Vector3(pathPoint.x + xOffset, pathPoint.y + yOffset, pathPoint.z);
            offsetPoints.Add(offsetPoint);
        }
        return offsetPoints;
    }

    // Given a blueprint/requirements, return a Unit of said blueprint.
    public GameObject CreateUnit(string unitType, List<Vector3> unitPathPoints, string unitTeam, Transform spawnerTransform)
    {
        GameObject unitPrefab = GetPrefabFromUnitString(unitType);
        GameObject unitInstance = Instantiate(unitPrefab, spawnerTransform.transform.position, spawnerTransform.rotation);

        spawnedUnitStats.ResetToStartingStats(unitType);

        unitInstance.GetComponent<Unit>().testMode_noPossession = false;

        unitInstance.GetComponent<Unit>().Initalize(OffsetUnitPathPoints(unitPathPoints), unitTeam, spawnedUnitStats);
        unitInstance.GetComponent<MeshRenderer>().material = GetTeamMaterial(unitTeam);

        return unitInstance;
    }

    Material GetTeamMaterial(string team)
    {
        Material teamMat = (team == "RED") ? redMat : blueMat;

        return teamMat;
    }
}
