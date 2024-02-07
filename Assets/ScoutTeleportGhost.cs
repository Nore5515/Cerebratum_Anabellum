using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTeleportGhost : MonoBehaviour
{
    [SerializeField]
    float lifetime = 1.0f;

    [SerializeField]
    SpriteRenderer scoutSprite;

    float r, g, b;

    float a;

    // Start is called before the first frame update
    void Start()
    {
        r = scoutSprite.color.r;
        g = scoutSprite.color.g;
        b = scoutSprite.color.b;
        a = scoutSprite.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        scoutSprite.color = new Color(r, g, b, a -= Time.deltaTime);
        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
