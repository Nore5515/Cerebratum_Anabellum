using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRighter : MonoBehaviour
{

    Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 1);
    public bool flipped = false;

    public void Flip(){
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = !sr.flipX;
        }
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
