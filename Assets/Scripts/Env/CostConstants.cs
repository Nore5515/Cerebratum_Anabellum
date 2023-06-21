public static class CostConstants
{
    public static int INF_SPAWNER_COST { get; set; }

    public static float BOT_CREATE_INF_SPAWNER_CHANCE { get; set; }
    public static float BOT_CHANGE_SPAWNER_PATH_CHANCE { get; set; }

    static CostConstants()
    {
        INF_SPAWNER_COST = 1;
        BOT_CREATE_INF_SPAWNER_CHANCE = 0.5f;
        BOT_CHANGE_SPAWNER_PATH_CHANCE = 0.25f;
    }

}