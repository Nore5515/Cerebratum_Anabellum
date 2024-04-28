using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryGibExplosion : MonoBehaviour
{
    [SerializeField]
    GameObject gibPrefab;
    [SerializeField]
    List<Sprite> gibPrefabs = new List<Sprite>();

    [SerializeField]
    int minGibs = 3;

    [SerializeField]
    int maxGibs = 7;

    // Start is called before the first frame update
    void Start()
    {
        int gibCount = Random.Range(minGibs, maxGibs);
        while (gibCount > 0)
        {
            GameObject gibObj = Instantiate(gibPrefab, transform.position, transform.rotation);
            gibObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetRandomGib();
            gibCount--;
        }
        Destroy(gameObject);
    }

    Sprite GetRandomGib()
    {
        if (gibPrefabs.Count > 0)
        {
            return gibPrefabs[Random.Range(0, gibPrefabs.Count)];
        }
        else
        {
            return null;
        }
    }
}
