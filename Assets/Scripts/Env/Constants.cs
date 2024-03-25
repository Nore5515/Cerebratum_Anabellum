using UnityEngine;
using UnityEngine.UI;

public static class Constants
{

    // SCOUT STUFF
    public static int FREE_SCOUT_LIMIT = 3;

    public static int VP_TO_VICTORY = 10;

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
    public static float CONTROLLED_MOVEMENT_MODIFIER = 2.5f;
    public static float CONTROLLED_FIRE_DELAY_MODIFIER = 0.3f;

    public static float PATH_FOLLOW_DIVERGENCE = 0.0f;

    public static int CYCLE_MAX = 100;

    public static float PLACEMENT_RANGE = 5.0f;

    public static float ZED_OFFSET = -0.1f;

    public static string RED_TEAM = "RED";
    public static string BLUE_TEAM = "BLUE";

    public static Color RED_GLOW_COLOR = new Color(255, 0, 0, 0.3f);
    public static Color BLUE_GLOW_COLOR = new Color(0, 0, 255, 0.3f);

    public static int MINIMUM_FRAMES_TO_BE_IDLE = 60;
    public static float MAX_IDLE_SECONDS = 10.0f;
    public static float MIN_DIST_TO_MOVEMENT_DEST = 0.3f;

    public static int ENGAGEMENT_SPHERE_RADIUS_MODIFIER = 2;

    // UNIT STATS

    // Infantry
    public static string INF_TYPE => "Infantry";

    public static int INF_DMG => 1;
    public static int INF_HP => 2;
    public static float INF_SPEED => 0.75f;

    public static float INF_INIT_FIRE_DELAY = 2.0f;
    public static float INF_INIT_RANGE = 3.0f;
    public static float INF_INIT_SPAWN_DELAY = 3.0f;

    public static float INF_MAX_FIRE_DELAY = 0.5f;
    public static float INF_MAX_RANGE = 6.0f;
    public static float INF_MAX_SPAWN_DELAY = 1.0f;

    // --

    public static float GRENADE_LOCAL_SCALE = 2.0f;
    public static float GRENADE_INITIAL_SCALE = 0.25f;
    public static float GRENADE_MINIMUM_RELEASE_SCALE = 1.0f;

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
    public static string SCOUT_TYPE => "Scout";

    public static int SCOUT_DMG => 1;
    public static int SCOUT_HP => 8;
    public static float SCOUT_SPEED => 4.0f;

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