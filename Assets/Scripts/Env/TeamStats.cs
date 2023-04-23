public static class TeamStats
{
    // Team HP
    public static int BlueHP {get; set;}
    public static int RedHP {get; set;}
    // public static bool GameStarted {get; set;}

    // Nanites
    public static int BluePoints {get; set;}
    public static int RedPoints {get; set;}
    
    // Nanites per minute
    public static int RedNaniteGain {get; set;}
    public static int BlueNaniteGain {get; set;}

    // Cycle length.
    public static int CycleLength {get; set;}

    static TeamStats()
    {
        BlueHP = 10;
        RedHP = 10;
    }
}