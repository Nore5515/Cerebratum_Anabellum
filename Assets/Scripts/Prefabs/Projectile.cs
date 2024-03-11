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

        public Props(string newTeam, int _damage)
        {
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

    private bool projectileIsExhausted = false;

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        checkUnit(other);
        checkTower(other);
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
        if (projectileIsExhausted) return;
        Unit unit = other.gameObject.GetComponent<Unit>();

        if (unit == null) return;
        if (unit.unitStats.unitTeam == "NIL") return;
        if (unit.unitStats.unitTeam == team) return;

        if (unit.DealDamage(damage) <= 0)
        {
            Destroy(other.gameObject);
        }

        projectileIsExhausted = true;

        Destroy(gameObject);
    }

    /// <summary>
    /// If a hostile projectile hits an allied tower, the tower will be dealt damage
    /// and the projectile will be destroyed.
    /// </summary>
    private void checkTower(Collider other)
    {
        if (projectileIsExhausted) return;
        TowerScript tower = other.gameObject.GetComponent<TowerScript>();

        if (tower == null) return;
        if (tower.unitStats.unitTeam == team) return;

        tower.DealDamage(1);

        projectileIsExhausted = true;

        Destroy(gameObject);
    }
}
