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

    // Given a blueprint/requirements, return a Unit of said blueprint.
    public GameObject CreateUnit(string unitType, List<GameObject> unitPathPoints, string unitTeam, Transform spawnerTransform)
    {
        GameObject unitPrefab = GetPrefabFromUnitString(unitType);
        GameObject unitInstance = Instantiate(unitPrefab, spawnerTransform.transform.position, spawnerTransform.rotation);

        spawnedUnitStats.ResetToStartingStats(unitType);

        unitInstance.GetComponent<Unit>().testMode_noPossession = false;
        unitInstance.GetComponent<Unit>().Initalize(unitPathPoints, unitTeam, spawnedUnitStats);
        unitInstance.GetComponent<MeshRenderer>().material = GetTeamMaterial(unitTeam);

        return unitInstance;
    }

    Material GetTeamMaterial(string team)
    {
        Material teamMat = (team == "RED") ? redMat : blueMat;

        return teamMat;
    }
}
