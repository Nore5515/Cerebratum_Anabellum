public static class TeamStats
{
    // Team HP
    public static int BlueVP { get; set; }
    public static int RedVP { get; set; }

    public static int BlueInfSpawners { get; set; }
    public static int RedInfSpawners { get; set; }

    public static int BlueBuildingSlots { get; set; }
    public static int RedBuildingSlots { get; set; }

    // Nanites
    public static int BluePoints { get; set; }
    public static int RedPoints { get; set; }

    // Nanites per minute
    public static int RedNaniteGain { get; set; }
    public static int BlueNaniteGain { get; set; }

    // Cycle length.
    public static int CycleLength { get; set; }

    static TeamStats()
    {
        BlueVP = 0;
        RedVP = 0;
        BlueInfSpawners = 1;
        RedInfSpawners = 1;
        BlueBuildingSlots = 3;
        RedBuildingSlots = 3;
    }

    public static void ResetGame()
    {
        BlueVP = 0;
        RedVP = 0;
        BluePoints = 0;
        RedPoints = 0;
        BlueNaniteGain = 0;
        RedNaniteGain = 0;
    }

    public static bool AttemptPointDeductionFromTeam(int pointDeduction, string teamToDeduct)
    {
        if (teamToDeduct == "RED")
        {
            if (RedPoints >= pointDeduction)
            {
                RedPoints -= pointDeduction;
                return true;
            }
        }
        else
        {
            if (BluePoints >= pointDeduction)
            {
                BluePoints -= pointDeduction;
                return true;
            }
        }
        return false;
    }
}