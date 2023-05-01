using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spidertank : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public StompSphere stomp;
    public GameObject spidertankBullet;
    public GameObject spidertankBulletShadow;
    public GameObject explosionAnim;

    public void CSpidertank()
    {
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
        threatLevel = 20;
        unitType = "Spidertank";

        stomp = this.gameObject.transform.Find("StompRadius").gameObject.GetComponent<StompSphere>();
        explosionAnim = this.gameObject.transform.Find("ExplosionAnim").gameObject;
        explosionAnim.SetActive(false);
    }

    void Start()
    {
        CSpidertank();
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
        // TODO: Make this better?
        if (stomp.alliedTeam != team)
        {
            stomp.alliedTeam = team;
        }
        MovementUpdate();
    }

    // Possessed
    public override void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject obj = GameObject.Instantiate(spidertankBullet, position, Quaternion.identity) as GameObject;
        obj.transform.LookAt(GetRandomAdjacentPosition(position, 0.0f));
        obj.GetComponent<Projectile>().Init(team, dmg);
        
    }

    // This will create the traditional shadow!
    void ShadowShot(Vector3 pos, GameObject pairedBullet)
    {
        pos.y += 0.05f;
        GameObject shadow = GameObject.Instantiate(spidertankBulletShadow, pos, Quaternion.identity) as GameObject;
        shadow.GetComponent<ShadowScript>().pairedBullet = pairedBullet;
    }


    public override void FireAtTransform(Transform trans)
    {
        if (trans != null){
            StartCoroutine(TriggerFireAnim());
            Vector3 newPos = trans.position;
            newPos.y += 30;
            GameObject obj = GameObject.Instantiate(spidertankBullet, newPos, Quaternion.identity) as GameObject;
            obj.transform.LookAt(GetRandomAdjacentPosition(trans.position, 0.0f));
            obj.GetComponent<Projectile>().Init(team, dmg);
            ShadowShot(trans.position, obj);
        }
    }

    IEnumerator TriggerFireAnim()
    {
        explosionAnim.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        explosionAnim.SetActive(false);
    }
}

