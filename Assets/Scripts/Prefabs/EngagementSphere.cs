using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngagementSphere : MonoBehaviour
{
    public string alliedTeam;
    public Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        //if (transform.parent.gameObject.GetComponent<Unit>() != null)
        //{
        //    alliedTeam = transform.parent.gameObject.GetComponent<Unit>().unitTeam;
        //    // Debug.Log("TEAM IS: " + alliedTeam);
        //    unit = transform.parent.gameObject.GetComponent<Unit>();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (alliedTeam == null) return;

        //AttemptAddUnitInRange(other);

        //AttemptAddHQInRange(other);
    }

    void AttemptAddUnitInRange(Collider other)
    {
        //if (other.gameObject.GetComponent<Unit>() == null) return;
        //if (other.gameObject.GetComponent<Unit>().unitTeam == null) return;
        //if (other.gameObject.GetComponent<Unit>().unitTeam == alliedTeam) return;

        //unit.AddTargetInRange(other.gameObject);
    }

    void AttemptAddHQInRange(Collider other)
    {
        //if (other.gameObject.GetComponent<HQObject>() == null) return;
        //if (other.gameObject.GetComponent<HQObject>().team == alliedTeam) return;
        //unit.AddTargetInRange(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        //unit.RemoveTargetInRange(other.gameObject);
    }
}
