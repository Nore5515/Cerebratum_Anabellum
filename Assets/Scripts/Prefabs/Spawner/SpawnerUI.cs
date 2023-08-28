using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

// THIS FITS ONTO THE UI OBJ OF THE SPAWNER PREFAB

// Parent Scripts:
//		Spawner.cs

public class SpawnerUI : MonoBehaviour
{

    public Button spawnrateButton;
    public Button firerateButton;
    public Button rangeButton;
    public Button pathButton;

    public Image rangeFill;
    public Image fireRateFill;
    public Image spawnFill;

    public Spawner parentSpawnerObj;

    public bool AttemptInitializeUI()
    {
        InitalizeUI();
        return true;
    }

    private void InitalizeUI()
    {
        GameObject ui = gameObject;
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject upgrades = ui.transform.Find("Upgrades").gameObject;

        SetButtonInstances(upgrades, ui);
        AddButtonListeners();

        SetFills(ui);

        ResetUpgradeFillAmounts();

        SetUIVisible(false);
    }

    private void SetButtonInstances(GameObject upgrades, GameObject ui)
    {
        spawnrateButton = upgrades.transform.Find("RSpawn").gameObject.GetComponent<Button>();
        firerateButton = upgrades.transform.Find("RFireRate").gameObject.GetComponent<Button>();
        rangeButton = upgrades.transform.Find("RRange").gameObject.GetComponent<Button>();
        pathButton = ui.transform.Find("RDraw").gameObject.GetComponent<Button>();
    }

    private void AddButtonListeners()
    {
        spawnrateButton.onClick.AddListener(delegate { parentSpawnerObj.AttemptUpgradeSpawnRate(); });
        firerateButton.onClick.AddListener(delegate { parentSpawnerObj.AttemptUpgradeFireRate(); });
        rangeButton.onClick.AddListener(delegate { parentSpawnerObj.AttemptUpgradeRange(); });
        pathButton.onClick.AddListener(delegate { parentSpawnerObj.EnableDrawable(); });
    }

    private void SetFills(GameObject ui)
    {
        GameObject infhutCanvas = ui.transform.Find("Canvas").gameObject;
        rangeFill = infhutCanvas.transform.Find("range").gameObject.GetComponent<Image>();
        fireRateFill = infhutCanvas.transform.Find("firerate").gameObject.GetComponent<Image>();
        spawnFill = infhutCanvas.transform.Find("spawn").gameObject.GetComponent<Image>();
    }

    private void ResetUpgradeFillAmounts()
    {
        rangeFill.fillAmount = 0;
        fireRateFill.fillAmount = 0;
        spawnFill.fillAmount = 0;
    }

    public void SetUIVisible(bool isVis)
    {
        if (spawnrateButton != null)
        {
            spawnrateButton.gameObject.SetActive(isVis);
            firerateButton.gameObject.SetActive(isVis);
            rangeButton.gameObject.SetActive(isVis);
            pathButton.gameObject.SetActive(isVis);
        }
        else
        {
            Debug.LogError("SPAWN RATE BUTTON");
        }
    }

    public bool GetUIVisible()
    {
        return spawnrateButton.gameObject.activeSelf;
    }

    public void HandleNewDrawableState(bool newDrawableState, PathManager pathManager)
    {
        if (newDrawableState)
        {
            pathButton.GetComponentInChildren<TMP_Text>().text = "DRAW PATH";
        }
        else
        {
            if (pathManager.pathSpheres.Count > 0)
            {
                pathButton.GetComponentInChildren<TMP_Text>().text = "RESET PATH";
            }
        }
    }

    public void UpdateSlider(ref Slider slider, PathManager pathManager, int maxPathLength)
    {
        slider.value = (1.0f) * pathManager.pathSpheres.Count / maxPathLength;
        if (IsPathLengthMaxed(pathManager, maxPathLength))
        {
            TintSliderRed(ref slider);
        }
    }

    private bool IsPathLengthMaxed(PathManager pathManager, int maxPathLength)
    {
        return pathManager.pathSpheres.Count == maxPathLength;
    }


    private void TintSliderRed(ref Slider slider)
    {
        Color red = new Color(233f / 255f, 80f / 255f, 55f / 255f);
        slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = red;
    }

    public void RecalculateFill(SpawnedUnitStats spawnedUnitStats)
    {
        spawnFill.fillAmount = calculateFill(spawnedUnitStats.startingSpawnTime, 0.5f, spawnedUnitStats.spawnTime);
        fireRateFill.fillAmount = calculateFill(spawnedUnitStats.startingFireDelay, 0.25f, spawnedUnitStats.fireDelay);
        rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
    }

    float calculateFill(float min, float max, float target)
    {
        float diff = (max - min);
        float result = (target - min) / diff;
        // Debug.Log("Min/Max: " + min + "/" + max + " with a target of " + target + " = " + result);
        return result;
    }

}

