using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompSphere : MonoBehaviour
{

    public string alliedTeam;
    public bool readyToStomp;

    private void OnTriggerEnter(Collider other)
    {
        if (readyToStomp)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                if (other.gameObject.GetComponent<Unit>().unitTeam != alliedTeam)
                {
                    // TODO: Fix unit destruction!!
                    other.gameObject.GetComponent<Unit>().DealDamage(99);
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
