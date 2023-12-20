public static class Constants
{
    public static float BOT_CREATE_INF_SPAWNER_CHANCE => 0.5f;
    public static float BOT_CHANGE_SPAWNER_PATH_CHANCE => 0.25f;
    public static float BOT_UPGRADE_CHANCE => 0.75f;

    public static float INF_MIN_SPAWN_TIME => 1.0f;
    public static float INF_MIN_FIRE_DELAY => 0.5f;
    //public static float INF_MAX_RANGE => 6.0f;

    public static float INF_SPAWN_TIME_UPGRADE_AMOUNT => -0.5f;
    public static float INF_FIRE_RATE_UPGRADE_AMOUNT => -0.25f;
    public static float INF_RANGE_UPGRADE_AMOUNT => 0.5f;

    public static float FIRST_FIRE_DELAY = 0.15f;
    public static float MISS_RANGE_RADIUS = 0.25f;
    public static float CONTROLLED_MISS_RADIUS = 0.0f;

    public static float PATH_FOLLOW_DIVERGENCE = 0.0f;

    public static int CYCLE_MAX = 30;

    // UNIT STATS

    // Infantry
    public static int INF_DMG => 1;
    public static int INF_HP => 2;
    public static float INF_SPEED => 0.75f;

    public static float INF_INIT_FIRE_DELAY = 2.0f;
    public static float INF_INIT_RANGE = 3.0f;
    public static float INF_INIT_SPAWN_DELAY = 3.0f;

    public static float INF_MAX_FIRE_DELAY = 0.5f;
    public static float INF_MAX_RANGE = 6.0f;
    public static float INF_MAX_SPAWN_DELAY = 1.0f;

    // Spider
    public static int SPIDER_DMG => 2;
    public static int SPIDER_HP => 20;
    public static int SPIDER_SPEED => 2;

    public static float SPIDER_INIT_FIRE_DELAY = 2.0f;
    public static float SPIDER_INIT_RANGE = 5.0f;
    public static float SPIDER_INIT_SPAWN_DELAY = 8.0f;

    public static float SPIDER_MAX_FIRE_DELAY = 1.0f;
    public static float SPIDER_MAX_RANGE = 10.0f;
    public static float SPIDER_MAX_SPAWN_DELAY = 4.0f;

    // Scouts
    public static int SCOUT_DMG => 1;
    public static int SCOUT_HP => 1;
    public static int SCOUT_SPEED => 8;

    public static float SCOUT_INIT_FIRE_DELAY = 2.0f;
    public static float SCOUT_INIT_RANGE = 3.0f;
    public static float SCOUT_INIT_SPAWN_DELAY = 3.0f;

    public static float SCOUT_MAX_FIRE_DELAY = 0.5f;
    public static float SCOUT_MAX_RANGE = 6.0f;
    public static float SCOUT_MAX_SPAWN_DELAY = 1.0f;

    // PRICING
    public static int SCOUT_SPAWNER_COST = 1;
    public static int INF_SPAWNER_COST => 1;
    public static int NANITE_GEN_COST = 1;
    public static int SPIDERTANK_SPAWNER_COST = 1;
}