using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;


public class Crate2D : MonoBehaviour
{
    class CaptureProgress
    {
        public string team;
        public int progress;

        public CaptureProgress(string _team, int _progress)
        {
            team = _team;
            progress = _progress;
        }
    }

    [SerializeField]
    SpriteRenderer crateSprite;

    [SerializeField]
    TextMeshProUGUI progressText;

    [SerializeField]
    float chanceToReactivatePerSecond = 0.05f;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    bool suspendedOnStart = false;

    [SerializeField]
    float baseCrateMovespeed = 100.0f;

    [SerializeField]
    float minimumHQDistance = 2.0f;

    public GameObject assignedScout = null;

    int redProgress = 0;
    int blueProgress = 0;
    CaptureProgress captureProgress = new CaptureProgress(Constants.RED_TEAM, 0);

    public List<Unit> capturingUnits = new List<Unit>();

    int MAX_CAPTURE = 500;
    int MAX_CAPTURE_GROWTH_RATE = 500;

    bool crateSuspended = false;
    int crateRespawnTimer = 0;

    Color redColor = new Color(1.0f, 0.8f, 0.8f);
    Color neutralColor = new Color(1.0f, 1.0f, 1.0f);
    Color blueColor = new Color(0.8f, 0.8f, 1.0f);


    Dictionary<string, Vector3> hqLocations = new Dictionary<string, Vector3>();

    private void Start()
    {
        PopulateHqLocations();
        if (suspendedOnStart)
        {
            SuspendCrate();
        }
        InvokeRepeating("OutputTime", 1f, 0.25f);  //1s delay, repeat every 1s
    }

    void PopulateHqLocations()
    {
        List<GameObject> hqObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
        foreach (GameObject hq in hqObjects)
        {
            hqLocations.Add(hq.GetComponent<HQObject>().team, hq.transform.position);
        }
        if (hqLocations.Count <= 0)
        {
            Debug.LogError("No HQs found by crate!");
        }
    }

    void OutputTime()
    {
        if (!crateSuspended)
        {
            if (captureProgress.progress != 0)
            {
                progressText.text = (captureProgress.team + ": " + captureProgress.progress).ToString();
            }
            else
            {
                progressText.text = "";
            }
            CheckUnitCapture();
        }
        else
        {
            if (crateRespawnTimer >= 4)
            {
                crateRespawnTimer = 0;
                float result = Random.Range(0.0f, 1.0f);
                if (result < chanceToReactivatePerSecond)
                {
                    UnsuspendCrate();
                    MAX_CAPTURE = MAX_CAPTURE + MAX_CAPTURE_GROWTH_RATE;
                }
            }
            else
            {
                crateRespawnTimer++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        crateSprite.transform.Rotate(new Vector3(0.0f, 0.0f, 0.1f));
    }

    private void RemoveNullUnits()
    {
        List<Unit> newUnits = new List<Unit>();
        foreach (Unit u in capturingUnits)
        {
            if (u != null)
            {
                newUnits.Add(u);
            }
        }
        capturingUnits = newUnits;
    }

    private void CheckUnitCapture()
    {
        RemoveNullUnits();
        if (capturingUnits.Count > 0)
        {
            //DrawLinesToAllUnits();
            DrawLineToAssignedScout();

            if (assignedScout != null)
            {
                MoveTowardsHQ(assignedScout.GetComponent<Scout>().unitStats.unitTeam);
            }
        }
        else
        {
            lineRenderer.gameObject.SetActive(false);
            crateSprite.color = neutralColor;
        }
    }

    Vector3 GetDirectionVector(Vector3 start, Vector3 end)
    {
        Vector3 newDest = end;
        Vector3 heading = newDest - start;
        float distance = heading.magnitude;
        return heading / distance;

    }

    void MoveTowardsHQ(string hqTeam)
    {
        if (hqLocations[hqTeam] != null)
        {
            Vector3 direction = GetDirectionVector(transform.position, hqLocations[hqTeam]);

            transform.Translate(direction * baseCrateMovespeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, hqLocations[hqTeam]) < minimumHQDistance)
            {
                UnpackCrateForTeam(hqTeam);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("HQ Team not found in Crate2D");
        }
    }

    private void DrawLineToAssignedScout()
    {
        if (assignedScout != null)
        {
            DrawLineToUnit(assignedScout.GetComponent<Scout>(), 0);
        }
    }

    private void DrawLinesToAllUnits()
    {
        int lineCount = 0;
        foreach (Unit u in capturingUnits)
        {
            if (u != null)
            {
                DrawLineToUnit(u, lineCount);
                lineCount += 2;
            }
        }
    }

    private void DrawLineToUnit(Unit u, int startIndex)
    {
        lineRenderer.gameObject.SetActive(true);
        Vector3 startPoint = transform.position;
        Vector3 endPoint = u.transform.position;
        lineRenderer.positionCount = startIndex + 2;
        lineRenderer.SetPosition(startIndex, startPoint);
        lineRenderer.SetPosition(startIndex + 1, endPoint);
    }

    private int CalculateUnitCaptureRate(Unit u)
    {
        int captureRate = 0;
        if (u.unitStats.unitType == Constants.SCOUT_TYPE)
        {
            captureRate = 10;
        }
        else
        {
            captureRate = 1;
        }
        if (u.beingControlled)
        {
            captureRate *= 2;
        }
        return captureRate;
    }

    private Dictionary<string, int> GetCaptureRates()
    {
        Dictionary<string, int> teamCaptureRates = new Dictionary<string, int>();

        foreach (Unit u in capturingUnits)
        {
            if (!teamCaptureRates.ContainsKey(u.unitStats.unitTeam))
            {
                teamCaptureRates.Add(u.unitStats.unitTeam, 0);
            }
            teamCaptureRates[u.unitStats.unitTeam] += CalculateUnitCaptureRate(u);

        }
        return teamCaptureRates;
    }

    private void SetCrateColor(string teamColor)
    {
        if (teamColor == Constants.RED_TEAM)
        {
            crateSprite.color = redColor;
        }
        else
        {
            crateSprite.color = blueColor;
        }
    }

    private void UnpackCrateForTeam(string team)
    {
        if (team == Constants.RED_TEAM)
        {
            TeamStats.RedVP++;
        }
        else
        {
            TeamStats.BlueVP++;
        }
        SuspendCrate();
    }

    private void CalculateTotalCaptureRate()
    {
        Dictionary<string, int> teamCaptureRates = GetCaptureRates();
        KeyValuePair<string, int> highestTeamCount = new KeyValuePair<string, int>("", -1);
        KeyValuePair<string, int> secondHighestTeamCount = new KeyValuePair<string, int>("", -1);
        foreach (KeyValuePair<string, int> kvp in teamCaptureRates)
        {
            if (kvp.Value > highestTeamCount.Value)
            {
                KeyValuePair<string, int> temp = highestTeamCount;
                highestTeamCount = kvp;
                secondHighestTeamCount = temp;
            }
        }

        if (highestTeamCount.Value > secondHighestTeamCount.Value)
        {
            SetCrateColor(highestTeamCount.Key);
            int teamValueDifference = highestTeamCount.Value - secondHighestTeamCount.Value;

            if (captureProgress.team == highestTeamCount.Key)
            {
                captureProgress.progress += teamValueDifference;
            }
            else
            {
                captureProgress.progress -= teamValueDifference;
            }

            if (captureProgress.progress > MAX_CAPTURE)
            {
                UnpackCrateForTeam(captureProgress.team);
            }
            else if (captureProgress.progress < 0)
            {
                captureProgress.progress = -captureProgress.progress;
                captureProgress.team = highestTeamCount.Key;
            }

        }
        else
        {
            // Neutral.
        }
    }

    private void SuspendCrate()
    {
        crateSprite.gameObject.SetActive(false);
        crateSuspended = true;
        captureProgress = new CaptureProgress(Constants.RED_TEAM, 0);
        capturingUnits = new List<Unit>();
        progressText.text = "";
    }

    private void UnsuspendCrate()
    {
        crateSuspended = false;
        crateSprite.gameObject.SetActive(true);
        captureProgress = new CaptureProgress(Constants.RED_TEAM, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "DetectionRadius")
        {
            Unit detectedUnit = other.transform.parent.gameObject.GetComponent<Unit>();
            if (detectedUnit != null)
            {
                if (!capturingUnits.Contains(detectedUnit))
                {
                    if (detectedUnit.unitStats.unitType == Constants.SCOUT_TYPE)
                    {
                        capturingUnits.Add(detectedUnit);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "DetectionRadius")
        {
            Unit detectedUnit = other.transform.parent.gameObject.GetComponent<Unit>();
            if (detectedUnit != null)
            {
                if (capturingUnits.Contains(detectedUnit))
                {
                    capturingUnits.Remove(detectedUnit);
                }
            }
        }
    }
}
