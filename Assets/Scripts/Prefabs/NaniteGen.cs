using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaniteGen : Structure
{
    private string team;

    //Dictionary<string, int> teamNaniteGain = new Dictionary<string, int>
    //{
    //    {"BLUE", TeamStats.BlueNaniteGain },
    //    {"RED", TeamStats.RedNaniteGain }
    //};

    public void InitializeWithTeam(string newTeam)
    {
        team = newTeam;
    }

    public void StartGen()
    {
        if (team == "BLUE")
        {
            TeamStats.BlueNaniteGain++;
        }
        else
        {
            TeamStats.RedNaniteGain++;
        }
    }

    public void StopGen()
    {
        if (team == "BLUE")
        {
            TeamStats.BlueNaniteGain--;
        }
        else
        {
            TeamStats.RedNaniteGain--;
        }
    }
}
