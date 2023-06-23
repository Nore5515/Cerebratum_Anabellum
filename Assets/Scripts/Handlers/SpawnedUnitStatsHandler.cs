// public class SpawnedUnitStatsHandler
// {
//     public float startingFireDelay = 2.0f;
//     public float startingUnitRange = 3.0f;
//     public float startingSpawnTime = 3.0f;
//     public float MAX_UNIT_FIRE_RATE = 0.5f;
//     public float MAX_UNIT_RANGE = 6.0f;
//     public float MAX_UNIT_SPAWN_RATE = 1.0f;
//     public float fireDelay;
//     public float unitRange;
//     public float spawnTime;

//     public void ResetToStartingStats()
//     {
//         fireDelay = startingFireDelay;
//         unitRange = startingUnitRange;
//         spawnTime = startingSpawnTime;
//     }

//     public void AttemptUpgradeSpawnRate()
//     {
//         if (spawnedUnitStats.spawnTime < MAX_UNIT_SPAWN_RATE) return;
//         if (deductTeamPoints(1))
//         {
//             UpgradeSpawnRate();
//         }
//     }

//     public void AttemptUpgradeFireRate()
//     {
//         if (spawnedUnitStats.fireDelay < MAX_UNIT_FIRE_RATE) return;
//         if (deductTeamPoints(1))
//         {
//             UpgradeFireRate();
//         }
//     }

//     public void AttemptUpgradeRange()
//     {
//         if (spawnedUnitStats.unitRange > MAX_UNIT_RANGE) return;
//         if (deductTeamPoints(1))
//         {
//             UpgradeRange();
//         }
//     }

//     private void UpgradeSpawnRate()
//     {
//         spawnedUnitStats.spawnTime += Constants.INF_SPAWN_TIME_UPGRADE_AMOUNT;
//         spawnFill.fillAmount = calculateFill(startingSpawnTime, 0.5f, spawnTime);
//         if (spawnedUnitStats.spawnTime < Constants.INF_MIN_SPAWN_TIME)
//         {
//             spawnrateButton.interactable = false;
//         }
//     }

//     private void UpgradeFireRate()
//     {
//         fireDelay += Constants.INF_FIRE_RATE_UPGRADE_AMOUNT;
//         fireRateFill.fillAmount = calculateFill(startingFireDelay, 0.25f, fireDelay);
//         if (spawnedUnitStats.fireDelay < Constants.INF_MIN_FIRE_DELAY)
//         {
//             firerateButton.interactable = false;
//         }
//     }

//     private void UpgradeRange()
//     {
//         spawnedUnitStats.unitRange += Constants.INF_RANGE_UPGRADE_AMOUNT;
//         rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
//         if (spawnedUnitStats.unitRange > Constants.INF_MAX_RANGE)
//         {
//             rangeButton.interactable = false;
//         }
//     }
// }
