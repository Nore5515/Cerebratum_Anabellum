using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorSphere : MonoBehaviour
{
    public bool unitInRange = false;

    public int unitsInRange = 0;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("New Thing");
        if (other.gameObject.transform.parent != null)
        {
            if (other.gameObject.transform.parent.gameObject.CompareTag("unit"))
            {
                //Debug.Log("New Unit");
                unitsInRange++;
                UpdateUnitInRange();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Bye Thin");
        Debug.Log(other.name);
        if (other.gameObject.transform.parent != null)
        {
            if (other.gameObject.transform.parent.gameObject.CompareTag("unit"))
            {
                //Debug.Log("Bye Unit");
                unitsInRange--;
                UpdateUnitInRange();
            }
        }
    }

    private void UpdateUnitInRange()
    {
        if (unitsInRange > 0)
        {
            unitInRange = true;
        }
        else
        {
            unitInRange = false;
        }
    }
}
