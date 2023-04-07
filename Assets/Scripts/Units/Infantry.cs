using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infantry : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public CubeMaker cm;

    public void CInfantry()
    {
        unitObj = this.gameObject;
        controlDirection = new Vector3(0, 0, 0);
        MaxDist = 1.4;
        MinDist = 1;
        
        // Core stat initialization
        hp = 1;
        maxHP = hp;
        dmg = 1;
        speed = 4;
        rof = 2f;
        threatLevel = 3;
    }

    void Start()
    {
        CInfantry();
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

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //
        //   ╔══════════════════════════════════════════════╗
        // ╔══════════════════════════════════════════════════╗
        // ║                                                  ║
        // ║  MOVEMENT                                        ║
        // ║                                                  ║
        // ╚══════════════════════════════════════════════════╝
        //   ╚══════════════════════════════════════════════╝
        //

        if (beingControlled == false)
        {
            // If dest exists
            if (Dest != null)
            {
                // Get movement direction.
                var heading = Dest.transform.position - this.transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;
                
                float distToDest = Vector3.Distance(transform.position, Dest.transform.position);
                if (distToDest >= MinDist)
                {
                    // Translate movement.
                    transform.Translate(direction * speed * Time.deltaTime);
                    if (distToDest <= MaxDist && removing == false)
                    {
                        removing = true;
                        if (Dest != null)
                        {
                            RemovePoint(Dest);
                        }
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
}

