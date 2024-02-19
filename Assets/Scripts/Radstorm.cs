using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radstorm : MonoBehaviour
{
    List<Unit> inRangeUnits = new List<Unit>();

    Vector3 dest;
    Vector3 origin;

    [SerializeField]
    float speed;

    float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("KillTick", 0.25f, 0.25f);  //0.25 delay, repeat every 0.25s
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(dest, origin, time);
        time += speed * Time.deltaTime;
        if (time >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    public void BeginJourney(Vector3 _dest)
    {
        dest = _dest;
        origin = transform.position;
    }

    public void BeginJourney(Vector3 _dest, float _speed)
    {
        speed = _speed;
        dest = _dest;
        origin = transform.position;
    }

    void KillTick()
    {
        RemoveNullUnits();
        foreach (Unit unit in inRangeUnits)
        {
            Destroy(unit.gameObject);
        }
    }

    private void RemoveNullUnits()
    {
        List<Unit> newUnits = new List<Unit>();
        foreach (Unit u in inRangeUnits)
        {
            if (u != null)
            {
                newUnits.Add(u);
            }
        }
        inRangeUnits = newUnits;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "UnitCollisionDetector")
        {
            Unit detectedUnit = other.transform.parent.gameObject.GetComponent<Unit>();
            if (detectedUnit != null)
            {
                if (!inRangeUnits.Contains(detectedUnit))
                {
                    inRangeUnits.Add(detectedUnit);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "UnitCollisionDetector")
        {
            Unit detectedUnit = other.transform.parent.gameObject.GetComponent<Unit>();
            if (detectedUnit != null)
            {
                if (inRangeUnits.Contains(detectedUnit))
                {
                    inRangeUnits.Remove(detectedUnit);
                }
            }
        }
    }
}
