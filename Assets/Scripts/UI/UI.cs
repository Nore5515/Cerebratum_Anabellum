using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text healthText;
    public Text nanitesText;
    public Text nanitesPerMinuteText;
    public Text gameOverText;
    public Image gameoverBGImage;
    bool gameEnding = false;

    [SerializeField] Button placeSpawner;
    bool placingSpawner = false;
    [SerializeField] GameObject spawnerGhost;
    GameObject newSpawnerGhost;
    [SerializeField] GameObject spawnerPrefab;

    int startingRedHP;

    // Start is called before the first frame update
    void Start()
    {
        startingRedHP = TeamStats.RedHP;
        nanitesText = GameObject.Find("NanitesText").GetComponent<Text>();
        //nanitesPerMinuteText = GameObject.Find("NaniteGainText").GetComponent<Text>();
        healthText.text = "RED: 10 --- BLUE: 10";
        nanitesText.text = "RED: 0 --- BLUE: 0";
        //nanitesPerMinuteText.text = "RED: 0 --- BLUE: 0";
        placeSpawner.onClick.AddListener(delegate { PlaceSpawnerClicked(); });
    }

    void PlaceSpawnerClicked()
    {
        Debug.Log("Place Spawner!");

        // Toggles On/Off
        if (placingSpawner)
        {
            placingSpawner = false;
            GameObject.Destroy(newSpawnerGhost);
            newSpawnerGhost = null;
        }
        else
        {
            placingSpawner = true;
        }


    }

    void PlaceSpawnerUpdateLoop()
    {
        // Get Mouse Position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // // If you hit nothing, exit.
        if (hit.collider == null) return;

        // Create ghost image on mouse position
        // // Create instance of sprite of spawner on mouse pos

        if (newSpawnerGhost == null)
        {
            Debug.Log("Instntiating!");
            newSpawnerGhost = Instantiate(spawnerGhost);
        }

        Debug.Log("Going");
        newSpawnerGhost.transform.position = MousePositionZeroZed();

        // if valid placement, turn green. otherwise turn red.

        // On click, create instance of spawner on mouse position
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newSpawner = Instantiate(spawnerPrefab);
            newSpawner.transform.position = newSpawnerGhost.transform.position;
            newSpawner.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    Vector3 MousePositionZeroZed()
    {
        Vector3 zeroZed = new Vector3();
        Vector3 screenToWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        zeroZed.x = screenToWorldPos.x;
        zeroZed.y = screenToWorldPos.y;
        zeroZed.z = -0.5f;
        return zeroZed;
    }

    double GetRedHPPercentage()
    {
        double result = TeamStats.RedHP / startingRedHP;
        result = result * 100;
        return result;
    }

    void Update()
    {

        if (placingSpawner)
        {
            PlaceSpawnerUpdateLoop();
        }

        healthText.text = GetRedHPPercentage().ToString() + "%";
        //healthText.text = "RED: " + TeamStats.RedHP.ToString() + " --- BLUE: " + TeamStats.BlueHP.ToString();
        nanitesText.text = TeamStats.RedPoints.ToString();
        //nanitesText.text = "RED: " + TeamStats.RedPoints.ToString() + " --- BLUE: " + TeamStats.BluePoints.ToString();
        //nanitesPerMinuteText.text = "RED: " + TeamStats.RedNaniteGain.ToString() + " --- BLUE: " + TeamStats.BlueNaniteGain.ToString();

        // if (TeamStats.GameStarted)
        // {
        if (TeamStats.BlueHP <= 0 || TeamStats.RedHP <= 0)
        {
            if (!gameEnding)
            {
                gameEnding = true;
                if (TeamStats.BlueHP > TeamStats.RedHP)
                {
                    gameOverText.text = "Blue Victory!";
                }
                else
                {
                    gameOverText.text = "Red Victory!";
                }
                gameoverBGImage.enabled = true;
                IEnumerator coroutine = EndGame();
                StartCoroutine(coroutine);
            }
        }
        // }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5.0f);
        //SpawnerTracker.NewGame();
        TeamStats.ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
