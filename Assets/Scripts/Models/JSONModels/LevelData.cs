using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public IList<LevelDetails> LevelsAvailable { get; set; }
}

public class LevelDetails
{
    public int LevelNumber { get; set; }
    public string LevelName { get; set; }
    public string LevelDesc { get; set; }
}