using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : MonoBehaviour
{

    public List<GameObject> targetsInRange = new List<GameObject>();
    public bool beingControlled = false;
    public bool canFire = true;
    public bool canFireDelay = false;

    public float fireDelay = 0.5f;

    public int health = 20;
    public int maxHealth = 20;

    public string team = "RED";
    public GameObject bullet;
    public GameObject HPBar;

    // Start is called before the first frame update
    void Start()
    {
        HPBar.GetComponent<Slider>().value = health;
        HPBar.GetComponent<Slider>().maxValue = health;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void DealDamage(int damage)
    {
        health -= 1;
        HPBar.GetComponent<Slider>().value = health;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void FireAtTransform(Transform trans)
    {
        GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetPositionNearTransform(trans, 1.0f));
        obj.GetComponent<Projectile>().Init(team);
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

}
