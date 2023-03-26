using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSphere : MonoBehaviour
{

    public string alliedTeam;
    public Unit unit;
    public TowerScript ts;

    public bool isUnit = false;
    public bool isTower = false;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.gameObject.GetComponent<Unit>() != null)
        {
            alliedTeam = transform.parent.gameObject.GetComponent<Unit>().team;
            unit = transform.parent.gameObject.GetComponent<Unit>();
            isUnit = true;
        }
        else if (transform.parent.gameObject.GetComponent<TowerScript>() != null)
        {
            alliedTeam = transform.parent.gameObject.GetComponent<TowerScript>().team;
            ts = transform.parent.gameObject.GetComponent<TowerScript>();
            isTower = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            if (other.gameObject.GetComponent<Unit>().team != alliedTeam)
            {
                if (isTower)
                {
                    ts.AddTargetInRange(other.gameObject);
                }
                else if (isUnit)
                {
                    unit.AddTargetInRange(other.gameObject);
                }
            }
        }
        if (other.gameObject.GetComponent<Spawner>() != null)
        {
            if (other.gameObject.GetComponent<Spawner>().team != alliedTeam)
            {
                if (isTower)
                {
                    ts.AddTargetInRange(other.gameObject);
                }
                else if (isUnit)
                {
                    unit.AddTargetInRange(other.gameObject);
                }
            }
        }
        if (other.gameObject.GetComponent<TowerScript>() != null)
        {
            if (other.gameObject.GetComponent<TowerScript>().team != alliedTeam)
            {
                if (isTower)
                {
                    ts.AddTargetInRange(other.gameObject);
                }
                else if (isUnit)
                {
                    unit.AddTargetInRange(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTower)
        {
            ts.RemoveTargetInRange(other.gameObject);
        }
        else if (isUnit)
        {
            unit.RemoveTargetInRange(other.gameObject);
        }
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
