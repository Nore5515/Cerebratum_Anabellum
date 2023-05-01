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
    public string unitType { get; set; }
    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public GameObject bullet {get; set;}
    public Vector3 controlDirection { get; set; }

    // STATE
    // TODO: Enum this?
    public string threatState {get; set;}
    
    // WALK - No threats of equal or higher level.
    // STAND - Threats of equal level, but not higher.
    // FLEE - Threats of higher level.

    string path = "Asset_Projectile";

    IDictionary<string, GameObject> UnitSprites = new Dictionary<string, GameObject>();

    // Targets Stuff
    public List<GameObject> targetsInRange = new List<GameObject>();

     // Firing Stuff
    public bool canFire {get; set;}
    public bool canFireDelay {get; set;}


    public void Initalize(List<GameObject> newObjs, string newTeam, float _rof, float unitRange)
    {
        bullet = Resources.Load(path) as GameObject;
        MaxDist = 1.4;
        MinDist = 1;
        team = newTeam;
        threatState = "WALK";
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
    // CALLED WHEN POSSESSED
    public virtual void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject obj = GameObject.Instantiate(bullet, unitObj.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetRandomAdjacentPosition(position, missRange));
        obj.GetComponent<Projectile>().Init(team, dmg);
    }

    // CALLED WHEN AI
    public virtual void FireAtTransform(Transform trans)
    {
        if (trans != null){
            GameObject obj = GameObject.Instantiate(bullet, unitObj.transform.position, Quaternion.identity) as GameObject;
            obj.transform.LookAt(GetRandomAdjacentPosition(trans.position, 1.0f));
            obj.GetComponent<Projectile>().Init(team, dmg);
        }
    }

    // While controlled, fire as fast as you want.
    public void ControlledFire(Vector3 target)
    {
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

    // Target Logic 
    public void AddTargetInRange(GameObject target)
    {
        targetsInRange.Add(target);
        ClearNullTargets();
        UpdateThreatState();
        // Sprite = Firing
        // if (UnitSprites["Shooting"] != null){
        if (UnitSprites.ContainsKey("Shooting")){
            DisableAllSprites();
            UnitSprites["Shooting"].SetActive(true);
        }
    }

    public void UpdateThreatState() 
    {
        int highestThreat = -1;
        foreach (GameObject target in targetsInRange)
        {
            if (target.GetComponent<Unit>() != null){
                if (target.GetComponent<Unit>().threatLevel > highestThreat)
                {
                    highestThreat = target.GetComponent<Unit>().threatLevel;
                }
            }
        }
        if (highestThreat < threatLevel)
        {
            threatState = "WALK";
        }
        else if (highestThreat == threatLevel)
        {
            threatState = "STAND";
        }
        else
        {
            threatState = "FLEE";
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
            ClearNullTargets();
        }
        if (targetsInRange.Count <= 0){
            // Sprite = Walking
            if (UnitSprites.ContainsKey("Walking"))
            {
                DisableAllSprites();
                UnitSprites["Walking"].SetActive(true);
            }
        }
    }
    
    public void ClearNullTargets()
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
        UpdateThreatState();
    }


    //
    //   ╔══════════════════════════════════════════════╗
    // ╔══════════════════════════════════════════════════╗
    // ║                                                  ║
    // ║  Point Logic                                     ║
    // ║                                                  ║
    // ╚══════════════════════════════════════════════════╝
    //   ╚══════════════════════════════════════════════╝
    //
    public void AddPoint(GameObject point)
    {
        objs.Add(point);
        if (objs.Count == 1){
            Dest = objs[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        objs.Remove(point);
        removing = false;
        if (objs.Count == 0)
        {
            Dest = null;
        }
        else 
        {
            Dest = objs[0];    
        }
    }

    // Takes in a position and a float (representing randomness)
    // Generates a random position up to randomness away
    // Returns it
    public Vector3 GetRandomAdjacentPosition(Vector3 position, float randomness)
    {
        float randomX = position.x + Random.Range(-randomness, randomness);
        float randomZ = position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, position.y, randomZ);
        return random;
    }


    //
    //   ╔══════════════════════════════════════════════╗
    // ╔══════════════════════════════════════════════════╗
    // ║                                                  ║
    // ║  MOVEMENT                                        ║
    // ║                                                  ║
    // ╚══════════════════════════════════════════════════╝
    //   ╚══════════════════════════════════════════════╝
    //
    public void AIMovement()
    {
        // If dest exists, cus otherwise you're just stayin' still.
        if (Dest != null)
        {
            // If an enemy is in range, stay still!
            // if (targetsInRange.Count > 0)
            if (threatState == "STAND" || threatState == "FLEE")
            {
                // Skirmish.
            }
            else
            {
                // Get movement direction.
                var heading = Dest.transform.position - this.transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;
                
                float distToDest = Vector3.Distance(transform.position, Dest.transform.position);
                
                // If you are not close enough to your dest, keep moving towards it.
                if (distToDest >= MinDist)
                {
                    // Translate movement.
                    transform.Translate(direction * speed * Time.deltaTime);
                }

                // Once you get too close to your destination, remove it from your movement path and go towards the next one.
                if (distToDest <= MaxDist)
                {
                    if (removing == false)
                    {
                        removing = true;
                        RemovePoint(Dest);
                    }
                }
            }
        }
    }

    public void PossessedMovement(){
        if (controlDirection != new Vector3(0, 0, 0))
        {
            // When controlled, move 50% faster.
            transform.Translate(controlDirection * (speed * 1.5f) * Time.deltaTime);
        }
    }

    public void AIFire() {
        if (canFire)
        {
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

    public void MovementUpdate() {
        if (beingControlled == false)
        {
            AIMovement();
            
            // If there's a valid target within range!
            if (targetsInRange.Count > 0){
            
                ClearNullTargets();

                // Are there any targets left after the purge?
                if (targetsInRange.Count > 0)
                {
                    AIFire();
                }
            }
        }
        else if (beingControlled)
        {
            PossessedMovement();
        }
    }
}