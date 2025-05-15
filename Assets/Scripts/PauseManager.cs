using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Assign in Inspector

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f; // Ensure time is normal at start
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ContinueGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /*void Update()
    {
        // Optional: pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ContinueGame();
            else
                PauseGame();
        }
    }*/
}
