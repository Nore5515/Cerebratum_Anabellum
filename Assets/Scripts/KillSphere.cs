using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSphere : MonoBehaviour
{

    public string alliedTeam;
    public GameObject bullet;
    public Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        alliedTeam = transform.parent.gameObject.GetComponent<Unit>().team;
        unit = transform.parent.gameObject.GetComponent<Unit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            if (other.gameObject.GetComponent<Unit>().team != alliedTeam)
            {
                // GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
                // obj.transform.LookAt(GetPositionNearTransform(other.gameObject.transform, 1.5f));
                // obj.GetComponent<Projectile>().Init(alliedTeam);
                unit.AddTargetInRange(other.gameObject);
            }
        }
        if (other.gameObject.GetComponent<Spawner>() != null)
        {
            if (other.gameObject.GetComponent<Spawner>().team != alliedTeam)
            {
                // GameObject obj = Instantiate(bullet, this.transform.position, Quaternion.identity) as GameObject;
                // obj.transform.LookAt(GetPositionNearTransform(other.gameObject.transform, 1.5f));
                // obj.GetComponent<Projectile>().Init(alliedTeam);
                unit.AddTargetInRange(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        unit.RemoveTargetInRange(other.gameObject);
    }

    private Vector3 GetPositionNearTransform(Transform trans, float randomness)
    {
        float randomX = trans.position.x + Random.Range(-randomness, randomness);
        float randomY = trans.position.y + Random.Range(-randomness, randomness);
        float randomZ = trans.position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, randomY, randomZ);
        return random;
    }
}
