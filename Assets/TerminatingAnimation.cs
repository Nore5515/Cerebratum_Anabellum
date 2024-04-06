using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminatingAnimation : MonoBehaviour
{
    Animator m_Animator;
    AnimatorClipInfo[] m_CurrentClipInfo;

    float m_CurrentClipLength;

    void Start()
    {
        //Get them_Animator, which you attach to the GameObject you intend to animate.
        m_Animator = gameObject.GetComponent<Animator>();
        //Fetch the current Animation clip information for the base layer
        m_CurrentClipInfo = this.m_Animator.GetCurrentAnimatorClipInfo(0);
        //Access the current length of the clip
        m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;

        StartCoroutine(SelfDestruct(m_CurrentClipLength));
    }

    IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
