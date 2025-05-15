using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public int currentEnergy = 5;
    public int maxEnergy = 10;
    public float energyRegenTime = 180f;
    private float energyTimer = 0f;

    public TextMeshProUGUI energyText;
    public TextMeshProUGUI regenTimeText;

    // 🔹 Loading Screen UI
    public GameObject loadingScreen;
    public TextMeshProUGUI loadingText;
    public Slider loadingSlider;

    private const string EnergyKey = "CurrentEnergy";
    private const string EnergyTimerKey = "EnergyTimer";

    void Start()
    {
        LoadEnergyData();
        UpdateEnergyDisplay();
        StartCoroutine(EnergyRegen());

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    private void UpdateEnergyDisplay()
    {
        energyText.text = $"{currentEnergy}/{maxEnergy}";
    }

    private void UpdateRegenTimeDisplay()
    {
        if (currentEnergy < maxEnergy)
        {
            float timeLeft = energyRegenTime - energyTimer;
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            regenTimeText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            regenTimeText.text = "00:00";
        }
    }

    private IEnumerator EnergyRegen()
    {
        while (true)
        {
            if (currentEnergy < maxEnergy)
            {
                energyTimer += Time.deltaTime;
                if (energyTimer >= energyRegenTime)
                {
                    energyTimer = 0f;
                    currentEnergy = Mathf.Min(currentEnergy + 1, maxEnergy);
                    UpdateEnergyDisplay();
                    SaveEnergyData();
                }
            }

            UpdateRegenTimeDisplay();
            yield return null;
        }
    }

    public void PlayGame()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;
            UpdateEnergyDisplay();
            SaveEnergyData();
            StartCoroutine(LoadGameAsync());
        }
        else
        {
            Debug.Log("Not enough energy to play!");
        }
    }

    private IEnumerator LoadGameAsync()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingSlider.value = progress;
            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (asyncLoad.progress >= 0.9f)
            {
                loadingSlider.value = 1f;
                loadingText.text = "100%";
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void SaveEnergyData()
    {
        PlayerPrefs.SetInt(EnergyKey, currentEnergy);
        PlayerPrefs.SetFloat(EnergyTimerKey, energyTimer);
        PlayerPrefs.Save();
    }

    private void LoadEnergyData()
    {
        currentEnergy = PlayerPrefs.GetInt(EnergyKey, 5);
        energyTimer = PlayerPrefs.GetFloat(EnergyTimerKey, 0f);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveEnergyData();
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
            LoadEnergyData();
    }

    private void OnApplicationQuit()
    {
        SaveEnergyData();
    }
}
