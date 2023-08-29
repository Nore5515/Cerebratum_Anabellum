using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Properties of projectile class.
    /// </summary>
    public class Props 
    {
        public string team;
        public int damage;

        public Props(string newTeam, int _damage) {
            team = newTeam;
            damage = _damage;
        }
    }

    [SerializeField]
    private float survivalTime = 5f;

    [SerializeField]
    private float moveSpeed = 16f;

    [SerializeField]
    private int damage = 1;

    public string team = "NIL";

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        checkUnit(other);
        checkTower(other);
        checkHQ(other);
    }

    /// <summary>
    /// Sets team and damage properties on projectile.
    /// </summary>
    public void SetProps(Props props)
    {
        team = props.team;
        damage = props.damage;
    }

    /// <summary>
    /// Destroys self.
    /// </summary>
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Update function that is called once per frame.
    /// </summary>
    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * transform.forward;
    }

    /// <summary>
    /// If projectile hits unit, deal damage.
    /// </summary>
    private void checkUnit(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();

        Debug.Log("1");
        if (unit == null) return;
        Debug.Log("2");
        if (unit.unitTeam == "NIL") return;
        Debug.Log("3");
        if (unit.unitTeam == team) return;
        Debug.Log("4");

        if (unit.DealDamage(damage) <= 0)
        {
            Debug.Log("5");
            Destroy(other.gameObject);
        }
        
        Destroy(gameObject);
    }

    private void checkHQ(Collider other)
    {
        HQObject otherSpawn = other.gameObject.GetComponent<HQObject>();

        if (otherSpawn == null) return;
        if (otherSpawn.team == team) return;

        if (otherSpawn.team == "RED")
        {
            TeamStats.RedHP -= 1;
        }
        else
        {
            TeamStats.BlueHP -= 1;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// If a hostile projectile hits an allied tower, the tower will be dealt damage
    /// and the projectile will be destroyed.
    /// </summary>
    private void checkTower(Collider other) 
    {
        TowerScript tower = other.gameObject.GetComponent<TowerScript>();

        if (tower == null) return;
        if (tower.unitTeam == team) return;
            
        tower.DealDamage(1);

        Destroy(gameObject);
    }
}
