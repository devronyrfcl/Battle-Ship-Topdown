using UnityEngine;

public class PlayerActivation : MonoBehaviour
{
    // Array of player models in the game scene
    public GameObject[] playerModels;

    void Start()
    {
        // Activate the correct player model based on the saved index from PlayerPrefs
        ActivateSelectedModel();
    }

    // Activate the selected player model in the game scene
    private void ActivateSelectedModel()
    {
        int selectedModelIndex = PlayerPrefs.GetInt("SelectedPlayerModel", 0);  // Default to 0 if not saved

        // Deactivate all player models and reset their tag
        foreach (GameObject model in playerModels)
        {
            model.SetActive(false);
            model.tag = "Untagged"; // Reset tag
        }

        // Activate the selected model and tag it as "Player"
        if (selectedModelIndex >= 0 && selectedModelIndex < playerModels.Length)
        {
            playerModels[selectedModelIndex].SetActive(true);
            playerModels[selectedModelIndex].tag = "Player"; // Set "Player" tag
        }
        else
        {
            Debug.LogError("Invalid model index!");
        }
    }
}
