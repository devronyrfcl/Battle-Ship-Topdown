using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  // Add this for scene management

public class StatsCounter : MonoBehaviour
{
    [Header("Stats")]
    public int earnedCoins = 0;
    public int enemiesDeadCount = 0;
    public int earnedTrophies = 0;
    public int totalTrophies = 0;  // Added total trophy tracking

    [Header("UI")]
    public TextMeshProUGUI finalTrophyCountText;  // Final trophy display
    public TextMeshProUGUI finalCoinAmountText;   // Final coin amount display
    public TextMeshProUGUI enemiesDeadText;       // Final enemies defeated display
    public TextMeshProUGUI finalDeathCountText;   // Final death count display

    private const string HighestTrophyKey = "HighestTrophy";
    private const string TotalTrophyKey = "TotalTrophy";  // New PlayerPrefs key for total trophies

    private void Start()
    {
        UpdateEnemyUI(); // Initialize UI
    }

    // Add coins during gameplay
    public void AddCoins(int amount)
    {
        earnedCoins += amount;
        Debug.Log("Coins earned: " + earnedCoins);
    }

    // Add enemy death count
    public void AddDeathCount()
    {
        enemiesDeadCount += 1;
        Debug.Log("Enemies defeated: " + enemiesDeadCount);
        UpdateEnemyUI();
    }

    // Update the in-game enemy dead TMP text
    private void UpdateEnemyUI()
    {
        if (enemiesDeadText != null)
        {
            enemiesDeadText.text = enemiesDeadCount.ToString(); // Only show the count
        }
    }

    // Call this when game over
    public void GameOver()
    {
        int deathBonusCoins = enemiesDeadCount * 10;    // 1 death = +10 coins
        int totalFinalCoins = earnedCoins + deathBonusCoins;

        earnedTrophies = enemiesDeadCount * 5;          // 1 kill = +5 trophies
        totalTrophies += earnedTrophies;                // Add earned trophies to total trophies

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(totalFinalCoins);
            Debug.Log("GameOver! Added " + totalFinalCoins + " coins to player's total.");
        }
        else
        {
            Debug.LogWarning("CurrencyManager instance not found!");
        }

        // Update Game Over UI with only essential data
        if (finalTrophyCountText != null)
            finalTrophyCountText.text = "" + totalTrophies;

        // Update Final Coin Amount Text
        if (finalCoinAmountText != null)
            finalCoinAmountText.text = "" + totalFinalCoins;

        // Update Final Death Count Text
        if (finalDeathCountText != null)
            finalDeathCountText.text = "" + enemiesDeadCount;

        // Save total trophies
        PlayerPrefs.SetInt(TotalTrophyKey, totalTrophies);
        PlayerPrefs.Save();

        // Check and Save Highest Trophy
        int highestTrophy = PlayerPrefs.GetInt(HighestTrophyKey, 0);
        if (totalTrophies > highestTrophy)
        {
            PlayerPrefs.SetInt(HighestTrophyKey, totalTrophies);
            PlayerPrefs.Save();
            highestTrophy = totalTrophies;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Loads the scene named "MainMenu"
    }
}
