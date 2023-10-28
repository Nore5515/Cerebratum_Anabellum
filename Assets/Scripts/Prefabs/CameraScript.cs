using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCam;
    public GameObject anchor;

    public GameObject followObj;

    public float MoveSpeed = 5.0f;

    private float SHIFT_SPEED = 25.0f;
    private float BASE_SPEED = 10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    Vector3 ParseInputToOrthoVector(float xMovement, float zMovement)
    {
        Vector3 moveVec = new Vector3();
        moveVec.x = xMovement;

        //moveVec.y = 0.0f;
        moveVec.y = zMovement;
        //moveVec.z = zMovement;

        //moveVec.y = zMovement;
        //moveVec.z = 0.0f;
        //moveVec = Quaternion.Euler(45, 45, 0) * moveVec;

        //moveVec.y = 0.0f;
        return moveVec;
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
            //Vector3 movement = new Vector3(xMovement, 0, zMovement);
            Vector3 movement = ParseInputToOrthoVector(xMovement, zMovement);
            anchor.transform.position += movement * MoveSpeed * Time.deltaTime;
            //this.transform.parent.position += movement * MoveSpeed * Time.deltaTime;
        }
        else
        {
            // mainCam.fieldOfView = 40;
            Vector3 cameraTiltCompensation = new Vector3(0.0f, 0.0f, -30.0f);
            Vector3 newPos = followObj.transform.position + cameraTiltCompensation;
            newPos.y = 20;
            mainCam.transform.position = newPos;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = SHIFT_SPEED;
        }
        else
        {
            MoveSpeed = BASE_SPEED;
        }

        ///////////////////////////////////////////////////
        //                                               //
        //               SCROLL                          //
        //                                               //
        ///////////////////////////////////////////////////
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mainCam.orthographicSize <= 64)
            {
                mainCam.orthographicSize += 1;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (mainCam.orthographicSize >= 5)
            {
                mainCam.orthographicSize -= 1;
            }
        }
    }
}
