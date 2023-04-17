using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCam;
    public GameObject anchor;

    public GameObject followObj;

    public float MoveSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (followObj == null)
        {
            // mainCam.fieldOfView = 80;
            mainCam.transform.position = anchor.transform.position;
            float zMovement = Input.GetAxis("Vertical");
            float xMovement = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(xMovement, 0, zMovement);
            anchor.transform.position += movement * MoveSpeed * Time.deltaTime;
        }
        else
        {
            // mainCam.fieldOfView = 40;
            Vector3 newPos = followObj.transform.position;
            newPos.y = 20;
            mainCam.transform.position = newPos;
        }

        ///////////////////////////////////////////////////
        //                                               //
        //               SCROLL                          //
        //                                               //
        ///////////////////////////////////////////////////
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mainCam.fieldOfView<=125)
            {
                mainCam.fieldOfView +=2;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (mainCam.fieldOfView >= 20)
            {
                mainCam.fieldOfView -=2;
            }
        }
    }
}
