using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject[] playerModels;
    public int[] modelPrices;

    public TextMeshProUGUI priceText;
    public GameObject buyButton;
    public GameObject selectButton;
    public GameObject selectedButton;  // <-- NEW: "Selected" button for already selected model
    public GameObject warningUI;

    private int selectedModelIndex = 0;

    private const string SelectedModelKey = "SelectedPlayerModel";
    private const string PurchasedModelKey = "PurchasedModel";

    void Start()
    {
        LoadSelectedModel();
        LoadPurchasedModels();
        UpdateUI();
        ActivateSelectedModel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwapModelLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwapModelRight();
        }
    }

    public void SelectPlayerModel()
    {
        SaveSelectedModel();
        UpdateUI(); // <-- CHANGED: Update UI after selecting
    }

    public void BuyPlayerModel()
    {
        int playerCoins = CurrencyManager.Instance.GetCoinCount();
        int modelPrice = modelPrices[selectedModelIndex];

        if (playerCoins >= modelPrice)
        {
            CurrencyManager.Instance.SubtractCoins(modelPrice);
            SavePurchasedModel();
            UpdateUI(); // <-- CHANGED: Update UI after buying
        }
        else
        {
            ShowWarning(true);
        }
    }

    public void SwapModelLeft()
    {
        selectedModelIndex = (selectedModelIndex - 1 + playerModels.Length) % playerModels.Length;
        UpdateUI();
        ActivateSelectedModel();
    }

    public void SwapModelRight()
    {
        selectedModelIndex = (selectedModelIndex + 1) % playerModels.Length;
        UpdateUI();
        ActivateSelectedModel();
    }

    private void ActivateSelectedModel()
    {
        foreach (GameObject model in playerModels)
        {
            model.SetActive(false);
        }

        if (selectedModelIndex >= 0 && selectedModelIndex < playerModels.Length)
        {
            playerModels[selectedModelIndex].SetActive(true);
        }
    }

    private void UpdateUI()
    {
        priceText.text = "Price: " + modelPrices[selectedModelIndex].ToString();
        bool isModelPurchased = PlayerPrefs.GetInt(PurchasedModelKey + selectedModelIndex, 0) == 1;
        int currentSelectedIndex = PlayerPrefs.GetInt(SelectedModelKey, 0);

        warningUI.SetActive(false);

        if (!isModelPurchased)
        {
            buyButton.SetActive(true);
            selectButton.SetActive(false);
            selectedButton.SetActive(false);

            int playerCoins = CurrencyManager.Instance.GetCoinCount();
            int modelPrice = modelPrices[selectedModelIndex];

            if (playerCoins < modelPrice)
            {
                ShowWarning(true);
            }
        }
        else
        {
            buyButton.SetActive(false);

            if (selectedModelIndex == currentSelectedIndex)
            {
                // This model is selected
                selectedButton.SetActive(true);
                selectButton.SetActive(false);
            }
            else
            {
                // This model is purchased but not selected
                selectButton.SetActive(true);
                selectedButton.SetActive(false);
            }
        }
    }

    private void ShowWarning(bool show)
    {
        warningUI.SetActive(show);
    }

    private void SaveSelectedModel()
    {
        PlayerPrefs.SetInt(SelectedModelKey, selectedModelIndex);
        PlayerPrefs.Save();
    }

    private void LoadSelectedModel()
    {
        selectedModelIndex = PlayerPrefs.GetInt(SelectedModelKey, 0);
    }

    private void SavePurchasedModel()
    {
        PlayerPrefs.SetInt(PurchasedModelKey + selectedModelIndex, 1);
        PlayerPrefs.Save();
    }

    private void LoadPurchasedModels()
    {
        for (int i = 0; i < playerModels.Length; i++)
        {
            if (PlayerPrefs.GetInt(PurchasedModelKey + i, 0) == 1)
            {
                // Already purchased
            }
        }
    }
}
