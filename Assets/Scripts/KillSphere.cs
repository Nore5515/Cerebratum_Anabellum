using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSphere : MonoBehaviour
{

    public string alliedTeam;

    // Start is called before the first frame update
    void Start()
    {
        alliedTeam = transform.parent.gameObject.GetComponent<Unit>().team;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            // Debug.Log("Unit Found");
            if (other.gameObject.GetComponent<Unit>().team != alliedTeam)
            {
                // Debug.Log("Destroyed enemy.");
                Destroy(other.gameObject);
            }
        }
    }
}
