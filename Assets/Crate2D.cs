using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crate2D : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer crateSprite;

    [SerializeField]
    TextMeshProUGUI progressText;

    [SerializeField]
    float chanceToReactivatePerSecond = 0.05f;

    int redProgress = 0;
    int blueProgress = 0;

    public List<Unit> capturingUnits = new List<Unit>();

    int MAX_CAPTURE = 500;

    bool crateSuspended = false;
    int crateRespawnTimer = 0;

    private void Start()
    {
        InvokeRepeating("OutputTime", 1f, 0.25f);  //1s delay, repeat every 1s
    }

    void OutputTime()
    {
        if (!crateSuspended)
        {
            progressText.text = (redProgress - blueProgress).ToString();
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
                    crateSuspended = true;
                    crateSprite.gameObject.SetActive(true);
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
            CalculateUnitCaptureRate();
        }
    }

    private void CalculateUnitCaptureRate()
    {
        int redCount = 0;
        int blueCount = 0;
        foreach (Unit u in capturingUnits)
        {
            if (u.unitTeam == "RED")
            {
                redCount++;
            }
            else
            {
                blueCount++;
            }
        }

        int differenceBetweenTeams;

        if (redCount > blueCount)
        {
            differenceBetweenTeams = redCount - blueCount;
            if (blueProgress > 0)
            {
                blueProgress -= differenceBetweenTeams;
                if (blueProgress < 0)
                {
                    blueProgress = 0;
                }
            }
            else
            {
                redProgress += differenceBetweenTeams;
                if (redProgress > MAX_CAPTURE)
                {
                    SuspendCrate();
                }
            }
        }
        else
        {
            differenceBetweenTeams = blueCount - redCount;
            blueProgress += differenceBetweenTeams;
            if (redProgress > 0)
            {
                redProgress -= differenceBetweenTeams;
                if (redProgress < 0)
                {
                    redProgress = 0;
                }
            }
            else
            {
                blueProgress += differenceBetweenTeams;
                if (blueProgress > MAX_CAPTURE)
                {
                    SuspendCrate();
                }
            }
        }
    }

    private void SuspendCrate()
    {
        crateSprite.gameObject.SetActive(false);
        crateSuspended = true;
        redProgress = 0;
        blueProgress = 0;
        capturingUnits = new List<Unit>();
        progressText.text = "";
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
