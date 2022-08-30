using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCam;
    public GameObject anchor;

    public GameObject followObj;

    public float MoveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (followObj == null)
        {
            mainCam.transform.position = anchor.transform.position;
            float zMovement = Input.GetAxis("Vertical");
            float xMovement = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(xMovement, 0, zMovement);
            anchor.transform.position += movement * MoveSpeed * Time.deltaTime;
        }
        else
        {
            Vector3 newPos = followObj.transform.position;
            newPos.y = 20;
            mainCam.transform.position = newPos;
        }
    }
}
