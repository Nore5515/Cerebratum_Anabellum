using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutSpawning : MonoBehaviour
{
    bool spawnerButtonClicked = false;
    [SerializeField] GameObject scoutGhostPrefab;
    [SerializeField] GameObject scoutPrefab;

    GameObject scoutGhostInstance;

    bool spawnerInRange = false;

    List<Unit> scoutsSpawned = new List<Unit>();

    public void ScoutSpawnButtonClicked()
    {
        spawnerButtonClicked = !spawnerButtonClicked;
    }

    private void Update()
    {
        if (spawnerButtonClicked)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null) return;

            if (scoutGhostInstance == null)
            {
                scoutGhostInstance = Instantiate(scoutGhostPrefab, transform.position, transform.rotation);
            }
            else
            {
                scoutGhostInstance.GetComponentInChildren<SpriteRenderer>().color = GetGhostColor();
                scoutGhostInstance.transform.position = MousePositionZeroZed();
                //if (Input.GetMouseButtonDown(0))
                //{
                //    SpawnScout(scoutGhostInstance.transform.position);
                //}
            }
        }
        else
        {
            if (scoutGhostInstance != null)
            {
                Destroy(scoutGhostInstance);
            }
        }
    }

    public bool SpawnScoutByTeam(Vector3 scoutPos, string team)
    {
        int teamScouts = GetTeamScoutCount(team);
        int teamPoints = GetTeamPoints(team);
        if (teamScouts < Constants.FREE_SCOUT_LIMIT)
        {
            SpawnNewScout(scoutPos, team);
            return true;
        }
        else if (teamPoints >= (teamScouts - (Constants.FREE_SCOUT_LIMIT - 1)))
        {
            MakeTeamPayCost(team, (teamScouts - (Constants.FREE_SCOUT_LIMIT - 1)));
            SpawnNewScout(scoutPos, team);
            return true;
        }
        else
        {
            return false;
        }
    }

    void MakeTeamPayCost(string team, int cost)
    {
        if (team == Constants.RED_TEAM)
        {
            TeamStats.RedPoints -= cost;
        }
        else if (team == Constants.BLUE_TEAM)
        {
            TeamStats.BluePoints -= cost;
        }
        else
        {
            Debug.LogError("TEAM NOT INCLUDED");
        }
    }

    int GetTeamScoutCount(string team)
    {
        if (team == Constants.RED_TEAM)
        {
            return TeamStats.RedScouts;
        }
        else if (team == Constants.BLUE_TEAM)
        {
            return TeamStats.BlueScouts;
        }
        else
        {
            Debug.LogError("TEAM NOT INCLUDED");
            return int.MinValue;
        }
    }

    int GetTeamPoints(string team)
    {
        if (team == Constants.RED_TEAM)
        {
            return TeamStats.RedPoints;
        }
        else if (team == Constants.BLUE_TEAM)
        {
            return TeamStats.BluePoints;
        }
        else
        {
            Debug.LogError("TEAM NOT INCLUDED");
            return int.MinValue;
        }
    }

    void IncrementTeamScouts(string team)
    {
        if (team == Constants.RED_TEAM)
        {
            TeamStats.RedScouts++;
        }
        else if (team == Constants.BLUE_TEAM)
        {
            TeamStats.BlueScouts++;
        }
        else
        {
            Debug.LogError("TEAM NOT INCLUDED");
        }
    }

    public void SpawnNewScout(Vector3 scoutPos, string team)
    {
        Debug.Log("Spawning new scout for team " + team);
        IncrementTeamScouts(team);
        GameObject newScout = Instantiate(scoutPrefab);
        newScout.transform.position = scoutPos;
        newScout.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newScout.GetComponent<Scout>().unitStats.unitTeam = team;
        SpawnedUnitStats sus = new SpawnedUnitStats();
        sus.ResetToStartingStats(Constants.SCOUT_TYPE);
        newScout.GetComponent<Scout>().Initalize(new List<Vector3>(), team, sus);
        scoutsSpawned.Add(newScout.GetComponent<Unit>());
        spawnerButtonClicked = false;
    }

    //public bool SpawnScout(Vector3 scoutPos)
    //{
    //    if (TeamStats.RedScouts < Constants.FREE_SCOUT_LIMIT)
    //    {
    //        TeamStats.RedScouts++;
    //        GameObject newScout = Instantiate(scoutPrefab);
    //        newScout.transform.position = scoutPos;
    //        newScout.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    //        scoutsSpawned.Add(newScout.GetComponent<Unit>());
    //        spawnerButtonClicked = false;
    //        return true;
    //    }
    //    else if (TeamStats.RedPoints >= TeamStats.RedScouts - 2)
    //    {
    //        TeamStats.RedPoints -= TeamStats.RedScouts - 2;
    //        TeamStats.RedScouts++;
    //        GameObject newScout = Instantiate(scoutPrefab);
    //        newScout.transform.position = scoutPos;
    //        newScout.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    //        scoutsSpawned.Add(newScout.GetComponent<Unit>());
    //        spawnerButtonClicked = false;
    //        return true;
    //    }
    //    return false;
    //}

    int GetCurrentScoutCount()
    {
        List<Unit> newScoutList = new List<Unit>();
        foreach (Unit scout in scoutsSpawned)
        {
            if (scout != null)
            {
                newScoutList.Add(scout);
            }
        }
        scoutsSpawned = newScoutList;
        return scoutsSpawned.Count;
    }

    void SpendRedPoints(int pointsToSpend)
    {
        if (TeamStats.RedPoints >= pointsToSpend)
        {
            TeamStats.RedPoints -= pointsToSpend;
        }
    }

    Color GetGhostColor()
    {
        List<GameObject> hqs = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
        if (hqs.Count > 0)
        {
            foreach (var hq in hqs)
            {
                if (hq.GetComponent<HQObject>() != null)
                {
                    if (hq.GetComponent<HQObject>().team == "RED")
                    {
                        if (Vector3.Distance(scoutGhostInstance.transform.position, hq.transform.position) > Constants.PLACEMENT_RANGE)
                        {
                            spawnerInRange = false;
                            return new Color(1.0f, 0.0f, 0.0f, 0.40f);
                        }
                    }
                }
            }
        }
        spawnerInRange = true;
        return new Color(0.34f, 0.83f, 0.40f, 0.40f);
    }

    Vector3 MousePositionZeroZed()
    {
        Vector3 zeroZed = new Vector3();
        Vector3 screenToWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        zeroZed.x = screenToWorldPos.x;
        zeroZed.y = screenToWorldPos.y;
        zeroZed.z = -0.5f;
        return zeroZed;
    }
}
