using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnevOne()
    {
        SceneManager.LoadScene("AI_Duel_Scene");
    }

    public void OnevThree()
    {
        SceneManager.LoadScene("AIScene");
    }

    public void NoAI()
    {
        SceneManager.LoadScene("SimpleGameScene");
    }
}
