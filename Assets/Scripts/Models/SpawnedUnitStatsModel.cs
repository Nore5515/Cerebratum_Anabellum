public class SpawnedUnitStats
{
    public float startingFireDelay = 2.0f;
    public float startingUnitRange = 3.0f;
    public float startingSpawnTime = 3.0f;
    public float MAX_UNIT_FIRE_RATE = 0.5f;
    public float MAX_UNIT_RANGE = 6.0f;
    public float MAX_UNIT_SPAWN_RATE = 1.0f;
    public float fireDelay;
    public float unitRange;
    public float spawnTime;

    public void ResetToStartingStats()
    {
        fireDelay = startingFireDelay;
        unitRange = startingUnitRange;
        spawnTime = startingSpawnTime;
    }
}