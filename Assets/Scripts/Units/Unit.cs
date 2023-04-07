using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour
{
    // Core unit stats
    public int hp { get; set;}
    public int maxHP { get; set;}
    public int dmg { get; set;}
    public int speed { get; set;}
    public float rof { get; set;}
    public int threatLevel { get; set;}

    // Range (REFACTOR TODO: TODO;)
    public double MaxDist = 1.6;
    public int MinDist = 1;

    // UI
    public Slider hpSlider;
    // public GameObject spriteRed;
    // public GameObject spriteBlue;

    public string team { get; set; }
    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public GameObject bullet {get; set;}
    public Vector3 controlDirection { get; set; }

    string path = "Asset_Projectile";

    IDictionary<string, GameObject> UnitSprites = new Dictionary<string, GameObject>();

    public void Initalize(List<GameObject> newObjs, string newTeam, float _rof, float unitRange)
    {
        bullet = Resources.Load(path) as GameObject;
        MaxDist = 1.4;
        MinDist = 1;
        team = newTeam;
        if (team == "RED")
        {
            // nothing
        }
        else
        {
            gameObject.GetComponentInChildren<SpriteRighter>().Flip();
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

        // Sprite Stuff
        UnitSprites.Add("Walking",transform.Find("SpriteRighter/Sprite").gameObject);
        UnitSprites.Add("Shooting",transform.Find("SpriteRighter/Sprite_shooting").gameObject);
    }

    // Firing Stuff
    public bool canFire {get; set;}
    public bool canFireDelay {get; set;}

    public IEnumerator EnableFiring()
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

    // Targets Stuff
    public List<GameObject> targetsInRange = new List<GameObject>();

    // Health and Damage Logic
    public int DealDamage(int damage)
    {
        hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = hp;
        }
        return hp;
    }

    // Point Stuff
    public List<GameObject> objs = new List<GameObject>();
    public GameObject Dest;
    public bool removing = false;

    // FireAt Logic
    public void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject obj = GameObject.Instantiate(bullet, unitObj.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearPosition(position, missRange));
        obj.GetComponent<Projectile>().Init(team, dmg);
    }

    public void FireAtTransform(Transform trans)
    {
        if (trans != null){
            GameObject obj = GameObject.Instantiate(bullet, unitObj.transform.position, Quaternion.identity) as GameObject;
            obj.transform.LookAt(GetPositionNearTransform(trans, 1.0f));
            obj.GetComponent<Projectile>().Init(team, dmg);
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

    // Target Logic 
    public void AddTargetInRange(GameObject target)
    {
        targetsInRange.Add(target);
        ClearTargets();
        // Sprite = Firing
        if (UnitSprites["Shooting"] != null){
            DisableAllSprites();
            UnitSprites["Shooting"].SetActive(true);
        }
    }

    public void DisableAllSprites(){
        foreach (string sprite in UnitSprites.Keys)
        {
            UnitSprites[sprite].SetActive(false);
        }
    }

    public void RemoveTargetInRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            ClearTargets();
        }
        if (targetsInRange.Count <= 0){
            // Sprite = Walking
            if (UnitSprites["Walking"] != null){
                DisableAllSprites();
                UnitSprites["Walking"].SetActive(true);
            }
        }
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


    // Point Logic
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


    // Getting Dest for Bullets
    public Vector3 GetPositionNearPosition(Vector3 position, float randomness)
    {
        float randomX = position.x + Random.Range(-randomness, randomness);
        float randomZ = position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, 0.5f, randomZ);
        return random;
    }

    public Vector3 GetPositionNearTransform(Transform trans, float randomness)
    {
        float randomX = trans.position.x + Random.Range(-randomness, randomness);
        float randomZ = trans.position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, 0.5f, randomZ);
        return random;
    }
}