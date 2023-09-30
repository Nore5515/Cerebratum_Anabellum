using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAnimModel : MonoBehaviour
{
    [SerializeField]
    float survivalTime;

    public float growthRate = 1.0f;
    public float fadeRate = 1.0f;
    Color fireballMat;

    // Start is called before the first frame update
    void Start()
    {
        fireballMat = this.GetComponent<Renderer>().material.color;
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    void Update()
    {
        this.transform.localScale += new Vector3(growthRate * Time.deltaTime, growthRate * Time.deltaTime, growthRate * Time.deltaTime);

        fireballMat = GetComponent<Renderer>().material.color;
        Color newColor = new Color(fireballMat.r, fireballMat.g, fireballMat.b, fireballMat.a -= fadeRate * Time.deltaTime);
        if (fireballMat.a > 0.01f)
        {
            GetComponent<Renderer>().material.color = newColor;
        }
        else
        {
            fireballMat.a = 0.01f;
            GetComponent<Renderer>().material.color = newColor;
        }
        Debug.Log(fireballMat.a);

    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }
}
