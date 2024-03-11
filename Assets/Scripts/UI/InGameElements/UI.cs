using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text vpText;

    public Text gameOverText;
    public Image gameoverBGImage;
    bool gameEnding = false;

    [SerializeField]
    bool debugMode = false;

    [SerializeField] Button placeSpawner;
    bool placingSpawner = false;
    [SerializeField] GameObject spawnerGhost;
    GameObject newSpawnerGhost;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] GameObject ai_spawnerPrefab;

    // Environmental Stuff
    [SerializeField] GameObject env_scoutSpawning;

    bool spawnerInRange = false;

    private GameObject blueHQ;

    Economy sceneEcon;

    [SerializeField]
    GameObject techTreePrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        blueHQ = GetBlueHQ();

        vpText.text = "RED: 0 --- BLUE: 0";

        placeSpawner.onClick.AddListener(delegate { PlaceSpawnerClicked(); });
        GameObject econObj = GameObject.FindGameObjectWithTag("econ");
        if (econObj != null)
        {
            sceneEcon = econObj.GetComponent<Economy>();
        }

    }

    public void PlaceScoutClicked()
    {
        ScoutSpawning scoutSpawner = FindObjectOfType<ScoutSpawning>();
        if (scoutSpawner != null)
        {
            scoutSpawner.ScoutSpawnButtonClicked();
        }
        else
        {
            Instantiate(env_scoutSpawning, transform.position, transform.rotation);
            PlaceScoutClicked();
        }
    }

    void PlaceSpawnerClicked()
    {
        // Toggles On/Off
        if (placingSpawner)
        {
            placingSpawner = false;
            Destroy(newSpawnerGhost);
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
            //Debug.Log("Instntiating!");
            newSpawnerGhost = Instantiate(spawnerGhost);
        }


        //Debug.Log("Going");
        newSpawnerGhost.transform.position = MousePositionZeroZed();

        // if valid placement, turn green. otherwise turn red.
        newSpawnerGhost.GetComponentInChildren<SpriteRenderer>().color = GetGhostColor();


        // On click, create instance of spawner on mouse position
        if (Input.GetMouseButtonDown(0))
        {
            if (TeamStats.RedPoints > 0 && spawnerInRange)
            {
                SpendRedPoints(1);
                GameObject newSpawner = Instantiate(spawnerPrefab);
                newSpawner.transform.position = newSpawnerGhost.transform.position;
                newSpawner.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void SpendRedPoints(int pointsToSpend)
    {
        if (TeamStats.RedPoints >= pointsToSpend)
        {
            TeamStats.RedPoints -= pointsToSpend;
        }
    }

    public void AI_Place_Spawner()
    {
        Debug.Log("Ai place");
        if (blueHQ != null)
        {
            Vector2 randomCircleOffset = Random.insideUnitCircle * Constants.PLACEMENT_RANGE;
            Vector3 randomCircleOffsetZeroZ = new Vector3(randomCircleOffset.x, randomCircleOffset.y, -0.01f);
            GameObject newSpawner = Instantiate(ai_spawnerPrefab);
            newSpawner.transform.position = blueHQ.transform.position + randomCircleOffsetZeroZ;
            newSpawner.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newSpawner.GetComponent<Spawner>().spawnerTeam = "BLUE";
        }
        else
        {
            if (!debugMode)
            {
                Debug.LogError("Blue HQ not found -> UI");
            }
        }
    }

    GameObject GetBlueHQ()
    {
        List<GameObject> hqs = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
        if (hqs.Count > 0)
        {
            foreach (var hq in hqs)
            {
                if (hq.GetComponent<HQObject>() != null)
                {
                    if (hq.GetComponent<HQObject>().team == "BLUE")
                    {
                        return hq;
                    }
                }
            }
        }
        return null;
    }

    Color GetGhostColor()
    {
        List<GameObject> hqs = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
        if (hqs.Count > 0)
        {
            foreach (var hq in hqs)
            {
                if (hq.GetComponent<HQObject>() != null)
                {
                    if (hq.GetComponent<HQObject>().team == "RED")
                    {
                        if (Vector3.Distance(newSpawnerGhost.transform.position, hq.transform.position) > Constants.PLACEMENT_RANGE)
                        {
                            spawnerInRange = false;
                            return new Color(1.0f, 0.0f, 0.0f, 0.40f);
                        }
                    }
                }
            }
        }
        spawnerInRange = true;
        return new Color(0.34f, 0.83f, 0.40f, 0.40f);
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

    void Update()
    {

        if (placingSpawner)
        {
            PlaceSpawnerUpdateLoop();
        }

        vpText.text = TeamStats.BlueVP.ToString() + " vs " + TeamStats.RedVP.ToString();

        if (TeamStats.BlueVP >= Constants.VP_TO_VICTORY || TeamStats.RedVP >= Constants.VP_TO_VICTORY)
        {
            if (!gameEnding)
            {
                gameEnding = true;
                if (TeamStats.BlueVP > TeamStats.RedVP)
                {
                    gameOverText.text = "Blue Victory!";
                }
                else if (TeamStats.BlueVP < TeamStats.RedVP)
                {
                    gameOverText.text = "Red Victory!";
                }
                else
                {
                    gameOverText.text = "Tie?!";
                    Debug.LogError("TIE?!");
                }
                gameoverBGImage.enabled = true;
                IEnumerator coroutine = EndGame();
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5.0f);
        //SpawnerTracker.NewGame();
        TeamStats.ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
