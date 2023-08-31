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
        else if (unitType == "Spider")
        {
            fireDelay = Constants.SPIDER_INIT_FIRE_DELAY;
            unitRange = Constants.SPIDER_INIT_RANGE;
            spawnDelay = Constants.SPIDER_INIT_SPAWN_DELAY;
        }
        else if (unitType == "Scout")
        {
            fireDelay = Constants.SCOUT_INIT_FIRE_DELAY;
            unitRange = Constants.SCOUT_INIT_SPAWN_DELAY;
            spawnDelay = Constants.SCOUT_INIT_RANGE;
        }
        else
        {
            // Just default to infantry for now.
            fireDelay  = Constants.INF_INIT_FIRE_DELAY;
            unitRange  = Constants.INF_INIT_SPAWN_DELAY;
            spawnDelay = Constants.INF_INIT_RANGE;
        }
    }
}