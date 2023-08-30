public class SpawnedUnitStats
{
    public float fireDelay;
    public float unitRange;
    public float spawnDelay;

    public void ResetToStartingStats(string unitType)
    {
        if (unitType == "Infantry")
        {
            fireDelay = Constants.INF_INIT_FIRE_DELAY;
            unitRange = Constants.INF_INIT_RANGE;
            spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
        }
    }
}