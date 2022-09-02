using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float survivalTime = 5f;
    public float moveSpeed = 16f;

    public string team = "NIL";

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    public void Init(string newTeam)
    {
        team = newTeam;
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
            if (other.gameObject.GetComponent<Unit>().team != "NIL")
            {
                if (other.gameObject.GetComponent<Unit>().team != team)
                {
                    Debug.Log("Destroying " + other.gameObject.GetComponent<Unit>().team + "'s unit (via " + team + "'s projectile)");
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }
        if (other.gameObject.GetComponent<Spawner>() != null)
        {
            if (other.gameObject.GetComponent<Spawner>().team != team)
            {
                GameObject canvas = GameObject.Find("Canvas");
                canvas.GetComponent<UI>().DecrementHealth(other.gameObject);
                Destroy(this.gameObject);
            }
        }
        if (other.gameObject.GetComponent<TowerScript>() != null)
        {
            if (other.gameObject.GetComponent<TowerScript>().team != team)
            {
                other.gameObject.GetComponent<TowerScript>().DealDamage(1);
                Destroy(this.gameObject);
            }
        }
    }
}
