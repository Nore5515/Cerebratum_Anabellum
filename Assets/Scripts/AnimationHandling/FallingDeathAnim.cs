using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDeathAnim : MonoBehaviour
{
    public float spinSpeed = 0.04f;
    public float animTime = 2.0f;

    float timeElapsed = 0;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, spinSpeed));
        Vector3 shrinkedScale = transform.localScale;
        shrinkedScale *= 1 - (1 / animTime) * Time.deltaTime;
        transform.localScale = shrinkedScale;

        timeElapsed += Time.deltaTime;
        if (timeElapsed > (animTime))
        {
            Destroy(gameObject);
        }
    }
}
