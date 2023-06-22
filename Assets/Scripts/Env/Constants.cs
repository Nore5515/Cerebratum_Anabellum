public static class Constants
{
    public static int INF_SPAWNER_COST { get; set; }

    public static float BOT_CREATE_INF_SPAWNER_CHANCE { get; set; }
    public static float BOT_CHANGE_SPAWNER_PATH_CHANCE { get; set; }


    public static float INF_MIN_SPAWN_TIME { get; set; }
    public static float INF_MIN_FIRE_DELAY { get; set; }
    public static float INF_MAX_RANGE { get; set; }

    public static float INF_SPAWN_TIME_UPGRADE_AMOUNT { get; set; }
    public static float INF_FIRE_RATE_UPGRADE_AMOUNT { get; set; }
    public static float INF_RANGE_UPGRADE_AMOUNT { get; set; }



    static Constants()
    {
        INF_SPAWNER_COST = 1;
        BOT_CREATE_INF_SPAWNER_CHANCE = 0.5f;
        BOT_CHANGE_SPAWNER_PATH_CHANCE = 0.25f;

        INF_MIN_SPAWN_TIME = 1.0f;
        INF_MIN_FIRE_DELAY = 0.5f;
        INF_MAX_RANGE = 6.0f;

        INF_SPAWN_TIME_UPGRADE_AMOUNT = -0.5f;
        INF_FIRE_RATE_UPGRADE_AMOUNT = -0.25f;
        INF_RANGE_UPGRADE_AMOUNT = 0.5f;
    }

}