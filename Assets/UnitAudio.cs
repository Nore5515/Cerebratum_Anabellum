using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    const float pitchOffset = 0.2f;

    float originalPitch = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        originalPitch = audioSource.pitch;
    }

    public void PlayGunshotEffect()
    {
        audioSource.pitch = originalPitch + (originalPitch * Random.Range(-pitchOffset, pitchOffset));
        audioSource.Play();
    }
}
