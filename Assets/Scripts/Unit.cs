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

    public bool beingControlled = false;

    public Vector3 controlDirection = new Vector3(0, 0, 0);

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    public void Initalize(List<GameObject> newObjs, string newTeam, float _fireDelay, float unitRange)
    {
        team = newTeam;
        fireDelay = _fireDelay;
        KillSphere ks = GetComponentInChildren(typeof(KillSphere)) as KillSphere;
        ks.alliedTeam = team;
        ks.GetComponent<SphereCollider>().radius = unitRange;
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
        // When controlled, fire 50% faster.
        if (beingControlled)
        {
            yield return new WaitForSeconds(fireDelay * 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(fireDelay);
        }
        canFire = true;
        canFireDelay = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dest != null && beingControlled == false)
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
        else if (beingControlled)
        {
            if (controlDirection != new Vector3(0, 0, 0))
            {
                transform.LookAt(transform.position + controlDirection);
                // When controlled, move 25% faster.
                transform.position += transform.forward * (MoveSpeed * 1.25f) * Time.deltaTime;
            }
        }
        if (targetsInRange.Count > 0 && beingControlled == false)
        {
            ClearTargets();
            if (targetsInRange.Count > 0)
            {
                if (canFire)
                {
                    Debug.Log("Shots fired!");
                    canFire = false;
                    FireAtTransform(targetsInRange[0].gameObject.transform);
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

    public void ControlledFire(Vector3 target)
    {
        if (canFire)
        {
            Debug.Log("Shots fired!");
            canFire = false;
            FireAtPosition(target);
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

    public void FireAtPosition(Vector3 position)
    {
        GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearPosition(position, 1.0f));
        obj.GetComponent<Projectile>().Init(team);

    }

    public void FireAtTransform(Transform trans)
    {
        GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearTransform(trans, 1.0f));
        obj.GetComponent<Projectile>().Init(team);
    }

    private Vector3 GetPositionNearPosition(Vector3 position, float randomness)
    {
        float randomX = position.x + Random.Range(-randomness, randomness);
        // float randomY = position.y + Random.Range(-randomness, randomness);
        float randomZ = position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, 0.5f, randomZ);
        return random;
    }

    private Vector3 GetPositionNearTransform(Transform trans, float randomness)
    {
        float randomX = trans.position.x + Random.Range(-randomness, randomness);
        // float randomY = trans.position.y + Random.Range(-randomness, randomness);
        float randomZ = trans.position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, 0.5f, randomZ);
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