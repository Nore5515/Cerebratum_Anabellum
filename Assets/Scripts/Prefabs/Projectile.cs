using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float survivalTime = 5f;
    public float moveSpeed = 16f;
    public int damage = 1;

    public string team = "NIL";

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    public void Init(string newTeam, int _damage)
    {
        team = newTeam;
        damage = _damage;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            if (other.gameObject.GetComponent<Unit>().unitTeam != "NIL")
            {
                if (other.gameObject.GetComponent<Unit>().unitTeam != team)
                {
                    // Debug.Log("Destroying " + other.gameObject.GetComponent<Unit>().team + "'s unit (via " + team + "'s projectile)");
                    if (other.gameObject.GetComponent<Unit>().DealDamage(damage) <= 0)
                    {
                        Destroy(other.gameObject);
                    }
                    Destroy(this.gameObject);
                }
            }
        }
        // TODO: Fix this later...?
        Spawner otherSpawn = other.gameObject.GetComponent<Spawner>();
        if (otherSpawn != null)
        {
            if (otherSpawn.spawnerTeam != team)
            {
                if (otherSpawn.spawnerTeam == "RED")
                {
                    TeamStats.RedHP -= 1;
                }
                else
                {
                    TeamStats.BlueHP -= 1;
                }
                Destroy(this.gameObject);
            }
        }
        if (other.gameObject.GetComponent<TowerScript>() != null)
        {
            if (other.gameObject.GetComponent<TowerScript>().unitTeam != team)
            {
                other.gameObject.GetComponent<TowerScript>().DealDamage(1);
                Destroy(this.gameObject);
            }
        }
    }
}
