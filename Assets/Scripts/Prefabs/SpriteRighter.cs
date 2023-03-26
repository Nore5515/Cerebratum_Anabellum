using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRighter : MonoBehaviour
{

    Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 1);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.rotation != rot)
        {
            this.transform.rotation = rot;
        }
    }
}
