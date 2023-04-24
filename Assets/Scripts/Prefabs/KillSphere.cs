using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSphere : MonoBehaviour
{

    public string alliedTeam;
    public Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.gameObject.GetComponent<Unit>() != null)
        {
            alliedTeam = transform.parent.gameObject.GetComponent<Unit>().team;
            // Debug.Log("TEAM IS: " + alliedTeam);
            unit = transform.parent.gameObject.GetComponent<Unit>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO; BETTER Inialization safety
        if (alliedTeam != null)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                if (other.gameObject.GetComponent<Unit>().team != alliedTeam )
                {
                    if (other.gameObject.GetComponent<Unit>().team != null)
                    {
                        unit.AddTargetInRange(other.gameObject);
                    }
                    else
                    {
                        Debug.Log("FUCKIGN NULL TEAM: " + other.gameObject.name);
                    }
                }
            }
            if (other.gameObject.GetComponent<Spawner>() != null)
            {
                if (other.gameObject.GetComponent<Spawner>().team != alliedTeam)
                {
                    // Debug.Log("FOUND SPAWNER! I am " + alliedTeam + ", they are: " +other.gameObject.GetComponent<Spawner>().team);
                    unit.AddTargetInRange(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        unit.RemoveTargetInRange(other.gameObject);
    }

    private Vector3 GetPositionNearTransform(Transform trans, float randomness)
    {
        float randomX = trans.position.x + Random.Range(-randomness, randomness);
        float randomY = trans.position.y + Random.Range(-randomness, randomness);
        float randomZ = trans.position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, randomY, randomZ);
        return random;
    }
}
