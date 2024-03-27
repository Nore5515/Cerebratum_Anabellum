using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class FeedObj
{
    public string text;
    public float duration;

    public FeedObj(string text, float duration)
    {
        this.text = text;
        this.duration = duration;
    }
}

public class FeedScript : MonoBehaviour
{
    [SerializeField]
    float feedObjDuration = 5.0f;

    private TMP_Text m_TextComponent;

    List<FeedObj> feedObjs = new List<FeedObj>();

    // Update is called once per frame
    void Update()
    {
        if (StaticFeed.feed.Count > 0)
        {
            feedObjs.Add(new FeedObj(StaticFeed.feed.Dequeue(), feedObjDuration));
        }

        foreach (FeedObj obj in feedObjs)
        {
            obj.duration -= Time.deltaTime;
            if (obj.duration <= 0.0f)
            {
                feedObjs.Remove(obj);
            }
            else
            {

            }
        }
    }
}
