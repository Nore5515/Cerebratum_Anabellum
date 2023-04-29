using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    public float lifetime = 1.75f;
    public float shrinkRate = 1.00f;
    public GameObject pairedBullet;

    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        if (pairedBullet != null){
            float bulletHeight = pairedBullet.transform.position.y;
            if (bulletHeight != float.NaN && bulletHeight > 0.0f){
                this.transform.localScale = new Vector3(Mathf.Log(bulletHeight+1, 2.0f), 0.0f, Mathf.Log(bulletHeight+1, 2.0f));
                float newAlpha = (255 - (255 * (Mathf.Log(bulletHeight+1, 1.5f) / 10))) / 255.0f;
                Color c = new Color(0.0f, 0.0f, 0.0f, newAlpha);
                this.transform.Find("Circle").gameObject.GetComponent<SpriteRenderer>().color = c;
            }
            else{
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
