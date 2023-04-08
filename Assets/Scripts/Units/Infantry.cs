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

    // void AIMovement()
    // {
    //     // If dest exists, cus otherwise you're just stayin' still.
    //     if (Dest != null)
    //     {
    //         // Get movement direction.
    //         var heading = Dest.transform.position - this.transform.position;
    //         var distance = heading.magnitude;
    //         var direction = heading / distance;
            
    //         float distToDest = Vector3.Distance(transform.position, Dest.transform.position);
            
    //         // If you are not close enough to your dest, keep moving towards it.
    //         if (distToDest >= MinDist)
    //         {
    //             // Translate movement.
    //             transform.Translate(direction * speed * Time.deltaTime);
    //         }

    //         // Once you get too close to your destination, remove it from your movement path and go towards the next one.
    //         if (distToDest <= MaxDist)
    //         {
    //             if (removing == false)
    //             {
    //                 removing = true;
    //                 RemovePoint(Dest);
    //             }
    //         }
    //     }
    // }

    // void PossessedMovement(){
    //     if (controlDirection != new Vector3(0, 0, 0))
    //     {
    //         transform.LookAt(transform.position + controlDirection);
    //         // When controlled, move 50% faster.
    //         transform.Translate(controlDirection * (speed * 1.5f) * Time.deltaTime);
    //     }
    // }

    // void AIFire() {
    //     if (canFire)
    //     {
    //         canFire = false;
    //         FireAtTransform(targetsInRange[0].gameObject.transform);
    //     }
    //     else
    //     {
    //         if (canFireDelay == false)
    //         {
    //             canFireDelay = true;
    //             StartCoroutine(EnableFiring());
    //         }
    //     }
    // }

    

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
    }
}

