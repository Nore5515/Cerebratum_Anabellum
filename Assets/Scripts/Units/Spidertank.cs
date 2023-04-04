using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spidertank : MonoBehaviour, Unit
{
    // Core unit stats
    public int hp { get; set;}
    public int maxHP;
    public int dmg { get; set;}
    public int speed { get; set;}
    public float rof { get; set;}
    public int threatLevel { get; set;}

    public GameObject Dest;
    public double MaxDist = 1.6;
    public int MinDist = 1;
    public bool removing = false;
    public float survivalTime = 15.0f;

    public CubeMaker cm;

    // public string team = "RED";
    public string team { get; set; }
    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public Vector3 controlDirection { get; set; }

    // public Vector3 controlDirection = new Vector3(0, 0, 0);
    public GameObject bullet;

    public List<GameObject> objs = new List<GameObject>();
    public List<GameObject> targetsInRange = new List<GameObject>();

    public bool canFire = true;
    public bool canFireDelay = false;


    public GameObject spriteRed;
    public GameObject spriteBlue;

    public Slider hpSlider;

    public Spidertank(string _team)
    {
        team = _team;
        unitObj = this.gameObject;
        controlDirection = new Vector3(0, 0, 0);
        MaxDist = 1.4;
        MinDist = 1;

        // Core stat initialization
        hp = 20;
        maxHP = hp;
        dmg = 1;
        speed = 2;
        rof = 2f;
    }


    void Start()
    {
        hp = maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = hp;
        }
        if (survivalTime > 0)
        {
            IEnumerator coroutine = SelfDestruct();
            StartCoroutine(coroutine);
        }
    }

    public void Initalize(List<GameObject> newObjs, string newTeam, float _rof, float unitRange)
    {
        MaxDist = 1.4;
        MinDist = 1;
        team = newTeam;
        if (team == "RED")
        {
            spriteBlue.SetActive(false);
        }
        else
        {
            spriteRed.SetActive(false);
        }
        rof = _rof;
        KillSphere ks = GetComponentInChildren(typeof(KillSphere)) as KillSphere;
        ks.alliedTeam = team;
        ks.GetComponent<SphereCollider>().radius = unitRange;
        // Debug.Log(ks.alliedTeam);
        foreach (GameObject obj in newObjs)
        {
            objs.Add(obj);
        }
        if (objs.Count > 0)
        {
            Dest = objs[0];
        }
    }

    public int DealDamage(int damage)
    {
        hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = hp;
        }
        return hp;
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
            yield return new WaitForSeconds(rof * 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(rof);
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
                transform.position += transform.forward * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, Dest.transform.position) <= MaxDist && removing == false)
                {
                    Debug.Log("Yep yep!");
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
                transform.position += transform.forward * (speed * 1.25f) * Time.deltaTime;
            }
        }
        if (targetsInRange.Count > 0 && beingControlled == false)
        {
            ClearTargets();
            if (targetsInRange.Count > 0)
            {
                if (canFire)
                {
                    // Debug.Log("Shots fired!");
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
            // Debug.Log("Shots fired!");
            canFire = false;
            // Fire with perfect accuracy if controlled.
            if (beingControlled)
            {
                FireAtPosition(target, 0.0f);
            }
            else
            {
                FireAtPosition(target, 1.0f);
            }
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

    public void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearPosition(position, missRange));
        obj.GetComponent<Projectile>().Init(team, dmg);

    }

    public void FireAtTransform(Transform trans)
    {
        GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearTransform(trans, 1.0f));
        obj.GetComponent<Projectile>().Init(team, dmg);
    }

    public Vector3 GetPositionNearPosition(Vector3 position, float randomness)
    {
        float randomX = position.x + Random.Range(-randomness, randomness);
        // float randomY = position.y + Random.Range(-randomness, randomness);
        float randomZ = position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, 0.5f, randomZ);
        return random;
    }

    public Vector3 GetPositionNearTransform(Transform trans, float randomness)
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
        // Debug.Log(targetsInRange.Count);
    }

    public void RemoveTargetInRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            ClearTargets();
            // Debug.Log(targetsInRange.Count);
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
        Debug.Log("Removing point!");
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

