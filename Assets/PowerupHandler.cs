using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class PowerupHandler : MonoBehaviour
{
    public GameObject fireballPlaceholder;
    public GameObject fireballAnim;
    public GameObject fireWhisp;

    Color fireballMat;

    bool releasedMouse = false;

    // Start is called before the first frame update
    void Start()
    {
        fireballMat = fireballPlaceholder.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hitPos = RayHitFloorPosition();
        if (hitPos != new Vector3(0.0f, 0.0f, 0.0f))
        {
            fireballPlaceholder.transform.position = hitPos;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //fireballMat.a = 0.8f;
            //fireballPlaceholder.GetComponent<Renderer>().material.color = fireballMat;
            if (releasedMouse)
            {
                Instantiate(fireballAnim, hitPos, transform.rotation);
                VaporizePoorFools();
                releasedMouse = false;
            }
        }
        else
        {
            fireballMat.a = 0.2f;
            fireballPlaceholder.GetComponent<Renderer>().material.color = fireballMat;
            releasedMouse = true;
        }
    }

    void VaporizePoorFools()
    {
        Collider[] potentialVictims = GetAllCollidersInSphere();
        List<GameObject> confirmedVictims = new List<GameObject>();
        foreach (var hitCollider in potentialVictims)
        {
            if (hitCollider.gameObject.tag == "unit")
            {
                confirmedVictims.Add(hitCollider.gameObject);
            }
        }

        foreach (GameObject victim in confirmedVictims)
        {
            GameObject whispInstance = Instantiate(fireWhisp, victim.transform.position, victim.transform.rotation);
            whispInstance.transform.position += new Vector3(0.0f, 0.5f, 0.0f);
            if(victim.GetComponent<Unit>().DealDamage(999) <= 0)
            {
                Destroy(victim.GetComponent<Unit>().gameObject);
            }
        }
    }

    Collider[] GetAllCollidersInSphere()
    {
        Collider[] hitColliders = Physics.OverlapSphere(fireballPlaceholder.transform.position, 60);
        return hitColliders;
    }

    Vector3 RayHitFloorPosition()
    {
        RayObj rayObj = new RayObj();
        rayObj.ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayObj.hit = GetHitAgainstLayer(rayObj, "Floor");
        if (rayObj.hit.transform != null)
        {
            return rayObj.hit.transform.position;
        }
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    RaycastHit GetHitAgainstLayer(RayObj rayObj, string maskName)
    {
        RaycastHit caughtHit;
        Physics.Raycast(rayObj.ray, out caughtHit, Mathf.Infinity, GenerateLayerMask(maskName));
        return caughtHit;
    }

    LayerMask GenerateLayerMask(string maskName)
    {
        LayerMask mask = LayerMask.GetMask(maskName);
        return mask;
    }

}
