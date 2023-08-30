using UnityEngine;
using System.Collections;

public class SpawnerStatsHandler : MonoBehaviour
{

    string unitType;

    public void Initialize(string _unitType)
    {
        unitType = _unitType;
    }

	public bool IsMaxedFireRate(SpawnedUnitStats spawnedUnitStats)
	{
        if (unitType == "Infantry")
        {
            if (spawnedUnitStats.fireDelay < Constants.INF_MAX_FIRE_DELAY) return true;
        }
        return false;
    }

    public bool IsMaxedRange(SpawnedUnitStats spawnedUnitStats)
    {
        if (unitType == "Infantry")
        {
            if (spawnedUnitStats.unitRange > Constants.INF_MAX_RANGE) return true;
        }
        return false;
    }

    public bool IsMaxedSpawnRate(SpawnedUnitStats spawnedUnitStats)
    {
        if (unitType == "Infantry")
        {
            if (spawnedUnitStats.spawnDelay < Constants.INF_MAX_SPAWN_DELAY) return true;
        }
        return false;
    }

    public void UpgradeRange(SpawnedUnitStats spawnedUnitStats, SpawnerUI spawnerUI)
    {
        spawnedUnitStats.unitRange += Constants.INF_RANGE_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        // rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
        if (spawnedUnitStats.unitRange > Constants.INF_MAX_RANGE)
        {
            spawnerUI.rangeButton.interactable = false;
        }
    }

    public void UpgradeFireRate(SpawnedUnitStats spawnedUnitStats, SpawnerUI spawnerUI)
    {
        spawnedUnitStats.fireDelay += Constants.INF_FIRE_RATE_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        if (spawnedUnitStats.fireDelay < Constants.INF_MIN_FIRE_DELAY)
        {
            spawnerUI.firerateButton.interactable = false;
        }
    }

    public void UpgradeSpawnRate(SpawnedUnitStats spawnedUnitStats, SpawnerUI spawnerUI)
    {
        spawnedUnitStats.spawnDelay += Constants.INF_SPAWN_TIME_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        if (spawnedUnitStats.spawnDelay < Constants.INF_MIN_SPAWN_TIME)
        {
            spawnerUI.spawnrateButton.interactable = false;
        }
    }
}

