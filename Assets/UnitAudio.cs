using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    public void PlayGunshotEffect()
    {
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
