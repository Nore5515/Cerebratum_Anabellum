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
                if (Input.GetMouseButtonDown(0))
                {
                    if (GetCurrentScoutCount() < 1 && spawnerInRange)
                    {
                        GameObject newScout = Instantiate(scoutPrefab);
                        newScout.transform.position = scoutGhostInstance.transform.position;
                        newScout.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        scoutsSpawned.Add(newScout.GetComponent<Unit>());
                        spawnerButtonClicked = false;
                    }
                }
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
