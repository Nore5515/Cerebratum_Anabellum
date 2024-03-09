using System.Collections;
using System.Collections.Generic;
using System.Net;
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

    int startingRedHP;

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
        startingRedHP = TeamStats.RedHP;
        nanitesText = GameObject.Find("NanitesText").GetComponent<Text>();
        //nanitesPerMinuteText = GameObject.Find("NaniteGainText").GetComponent<Text>();
        healthText.text = "RED: 10 --- BLUE: 10";
        nanitesText.text = "RED: 0 --- BLUE: 0";
        //nanitesPerMinuteText.text = "RED: 0 --- BLUE: 0";
        placeSpawner.onClick.AddListener(delegate { PlaceSpawnerClicked(); });
        GameObject econObj = GameObject.FindGameObjectWithTag("econ");
        if (econObj != null)
        {
            sceneEcon = econObj.GetComponent<Economy>();
        }

        //InitializeTechTree();
    }

    void InitializeTechTree()
    {
        if (FindObjectOfType<TechTree>() == null)
        {
            GameObject instance = Instantiate(techTreePrefab, transform.position, transform.rotation);
            instance.transform.SetParent(transform);
            instance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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
        //Debug.Log("Place Spawner!");

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

    double GetRedHPPercentage()
    {
        double result = (double)TeamStats.RedHP / (double)startingRedHP;
        result *= 100;
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

        if (TeamStats.BluePoints > 0)
        {
            AI_Place_Spawner();
            TeamStats.BluePoints -= 1;
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
