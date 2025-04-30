using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // Energy variables
    public int currentEnergy = 5;  // Starting energy
    public int maxEnergy = 10;     // Max energy
    public float energyRegenTime = 180f; // 3 minutes for full regen (in seconds)
    private float energyTimer = 0f; // Timer to track energy regen

    // UI references (drag and drop the TextMeshPro elements in the inspector)
    public TextMeshProUGUI energyText; // Text to display energy (current/max)
    public TextMeshProUGUI regenTimeText; // Text to display time for next energy regen

    // PlayerPrefs keys for saving data
    private const string EnergyKey = "CurrentEnergy";
    private const string EnergyTimerKey = "EnergyTimer";

    void Start()
    {
        // Load saved energy and timer values
        LoadEnergyData();

        // Initialize energy and time display
        UpdateEnergyDisplay();
        StartCoroutine(EnergyRegen());
    }

    // Update energy display (current/max)
    private void UpdateEnergyDisplay()
    {
        energyText.text = $"{currentEnergy}/{maxEnergy}";
    }

    // Update the regeneration timer text
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

    // Handle energy regeneration over time
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

    // This method will be hooked to the Play button
    public void PlayGame()
    {
        if (currentEnergy > 0)  // Only allow playing if energy is available
        {
            // Decrease energy
            currentEnergy--;
            UpdateEnergyDisplay();

            // Switch to MainGame scene
            SceneManager.LoadScene("MainGame");

            // Save the energy data when playing
            SaveEnergyData();
        }
        else
        {
            // Optionally, show a message or sound if there's no energy
            Debug.Log("Not enough energy to play!");
        }
    }

    // Save the energy and timer to PlayerPrefs
    private void SaveEnergyData()
    {
        PlayerPrefs.SetInt(EnergyKey, currentEnergy);
        PlayerPrefs.SetFloat(EnergyTimerKey, energyTimer);
        PlayerPrefs.Save(); // Make sure to save the data
    }

    // Load the saved energy and timer from PlayerPrefs
    private void LoadEnergyData()
    {
        currentEnergy = PlayerPrefs.GetInt(EnergyKey, 5); // Default starting energy is 5
        energyTimer = PlayerPrefs.GetFloat(EnergyTimerKey, 0f); // Default starting timer is 0
    }

    // Call this function when the app is paused or going to background (e.g., OnApplicationPause)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Save energy when the app goes to the background
            SaveEnergyData();
        }
    }

    // Call this function when the app is resumed (e.g., OnApplicationFocus)
    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            // Load the energy data when the app is resumed
            LoadEnergyData();
        }
    }

    // Optional: Handle app quitting to ensure data is saved
    private void OnApplicationQuit()
    {
        SaveEnergyData();
    }
}
