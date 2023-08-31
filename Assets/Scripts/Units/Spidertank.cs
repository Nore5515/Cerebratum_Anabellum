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
        MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        hp = Constants.SPIDER_HP;
        maxHP = hp;
        dmg = Constants.SPIDER_DMG;
        speed = Constants.SPIDER_SPEED;
        rof = Constants.SPIDER_INIT_FIRE_DELAY;
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
        if (stomp.alliedTeam != unitTeam)
        {
            stomp.alliedTeam = unitTeam;
        }
        MovementUpdate();
    }

    // Possessed
    public override void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject obj = GameObject.Instantiate(spidertankBullet, position, Quaternion.identity) as GameObject;
        // TODO: i just commented this line out. I dont care.
        //obj.transform.LookAt(GetRandomAdjacentPosition(position, 0.0f));
        obj.GetComponent<Projectile>().SetProps(new Projectile.Props(unitTeam, dmg));
        //ShadowShot(position, obj);
        StartCoroutine(TriggerFireAnim());
        explosionAnim.gameObject.transform.position = position;
    }

    // This will create the traditional shadow!
    void ShadowShot(Vector3 pos, GameObject pairedBullet)
    {
        pos.y += 0.05f;
        GameObject shadow = GameObject.Instantiate(spidertankBulletShadow, pos, Quaternion.identity) as GameObject;
        shadow.GetComponent<ShadowScript>().pairedBullet = pairedBullet;
    }

    IEnumerator TriggerFireAnim()
    {
        explosionAnim.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        explosionAnim.SetActive(false);
    }
}

