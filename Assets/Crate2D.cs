using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


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
    bool suspendedOnStart = false;

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


    private void Start()
    {
        if (suspendedOnStart)
        {
            SuspendCrate();
        }
        InvokeRepeating("OutputTime", 1f, 0.25f);  //1s delay, repeat every 1s
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
            CalculateTotalCaptureRate();
        }
        else
        {
            crateSprite.color = neutralColor;
        }
    }

    private int CalculateUnitCaptureRate(Unit u)
    {
        int captureRate = 0;
        if (u.unitType == Constants.SCOUT_TYPE)
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
            if (!teamCaptureRates.ContainsKey(u.unitTeam))
            {
                teamCaptureRates.Add(u.unitTeam, 0);
            }
            teamCaptureRates[u.unitTeam] += CalculateUnitCaptureRate(u);

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
            TeamStats.RedPoints++;
        }
        else
        {
            TeamStats.BluePoints++;
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
                    capturingUnits.Add(detectedUnit);
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
