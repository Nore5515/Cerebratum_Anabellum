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

    int redProgress = 0;
    int blueProgress = 0;

    public List<Unit> capturingUnits = new List<Unit>();

    int MAX_CAPTURE = 500;

    private void Start()
    {
        InvokeRepeating("OutputTime", 1f, 0.25f);  //1s delay, repeat every 1s
    }

    void OutputTime()
    {
        progressText.text = (redProgress - blueProgress).ToString();
        CheckUnitCapture();
    }

    // Update is called once per frame
    void Update()
    {
        crateSprite.transform.Rotate(new Vector3(0.0f, 0.0f, 0.1f));
    }

    private void CheckUnitCapture()
    {
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
                    Destroy(gameObject);
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
                    Destroy(gameObject);
                }
            }
        }
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
