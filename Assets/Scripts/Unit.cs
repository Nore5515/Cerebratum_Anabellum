using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject Dest;
    int MoveSpeed = 4;
    double MaxDist = 1.2;
    int MinDist = 1;
    bool removing = false;
    public float survivalTime = 15.0f;

    public CubeMaker cm;

    public string team = "RED";

    public GameObject bullet;

    public List<GameObject> objs = new List<GameObject>();
    public List<GameObject> targetsInRange = new List<GameObject>();

    public float fireDelay = 2f;
    public bool canFire = true;
    public bool canFireDelay = false;

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    public void Initalize(List<GameObject> newObjs, string newTeam, float _fireDelay)
    {
        team = newTeam;
        fireDelay = _fireDelay;
        KillSphere ks = GetComponentInChildren(typeof(KillSphere)) as KillSphere;
        ks.alliedTeam = team;
        Debug.Log(ks.alliedTeam);
        foreach (GameObject obj in newObjs)
        {
            objs.Add(obj);
        }
        if (objs.Count > 0)
        {
            Dest = objs[0];
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }

    IEnumerator EnableFiring()
    {
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
        canFireDelay = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dest != null)
        {
            transform.LookAt(Dest.transform);

            if (Vector3.Distance(transform.position, Dest.transform.position) >= MinDist)
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, Dest.transform.position) <= MaxDist && removing == false)
                {
                    removing = true;
                    if (Dest != null)
                    {
                        RemovePoint(Dest);
                    }
                }

            }
        }
        if (targetsInRange.Count > 0)
        {
            ClearTargets();
            if (targetsInRange.Count > 0)
            {
                if (canFire)
                {
                    Debug.Log("Shots fired!");
                    canFire = false;
                    GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
                    obj.transform.LookAt(GetPositionNearTransform(targetsInRange[0].gameObject.transform, 1.5f));
                    obj.GetComponent<Projectile>().Init(team);
                }
                else
                {
                    if (canFireDelay == false)
                    {
                        canFireDelay = true;
                        StartCoroutine(EnableFiring());
                    }
                }
            }
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

    public void ClearTargets()
    {
        List<GameObject> toRemoveObjs = new List<GameObject>();
        foreach (GameObject obj in targetsInRange)
        {
            if (obj == null)
            {
                toRemoveObjs.Add(obj);
            }
        }
        foreach (GameObject markedUnit in toRemoveObjs)
        {
            targetsInRange.Remove(markedUnit);
        }
    }

    public void AddTargetInRange(GameObject target)
    {
        targetsInRange.Add(target);
        ClearTargets();
        Debug.Log(targetsInRange.Count);
    }
    public void RemoveTargetInRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            ClearTargets();
            Debug.Log(targetsInRange.Count);
        }
    }

    public void AddPoint(GameObject point)
    {
        objs.Add(point);
        if (Dest == null)
        {
            Dest = objs[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        objs.Remove(point);
        removing = false;
        if (Dest != null)
        {
            if (objs.Count <= 0)
            {
                Dest = null;
            }
            else
            {
                Dest = objs[0];
            }
        }
    }

}