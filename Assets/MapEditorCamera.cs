using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCamera : MonoBehaviour {

    public float MoveSpeed = 5.0f;

    private float SHIFT_SPEED = 30.0f;
    private float BASE_SPEED = 10.0f;

    // Update is called once per frame
    void Update()
    {
        float yMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(xMovement, yMovement, 0.0f);
        transform.position += movement * MoveSpeed * Time.deltaTime;

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
            if (Camera.main.orthographicSize <= 30)
            {
                Camera.main.orthographicSize += 1;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize >= 5)
            {
                Camera.main.orthographicSize -= 1;
            }
        }
    }
}
