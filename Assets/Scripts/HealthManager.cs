using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // For List

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public TMP_Text healthText;
    public Slider healthSlider;

    // Array to hold references to all enemies
    private List<MonoBehaviour> enemyGunManagers = new List<MonoBehaviour>();

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        AddEnemiesInScene();
    }

    // Method to add all enemies with the "Enemy" tag to the list
    void AddEnemiesInScene()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // Check for either EnemyGunShootManager or TankGunShootManager components
            EnemyGunShootManager enemyGunShootManager = enemy.GetComponent<EnemyGunShootManager>();
            TankGunShootManager tankGunShootManager = enemy.GetComponent<TankGunShootManager>();

            // Add the respective component to the list
            if (enemyGunShootManager != null)
            {
                enemyGunManagers.Add(enemyGunShootManager);
            }
            else if (tankGunShootManager != null)
            {
                enemyGunManagers.Add(tankGunShootManager);
            }
        }
    }

    // Call this to apply damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // If health reaches 0, stop shooting for all enemies
        if (currentHealth <= 0)
        {
            StopEnemyShooting();
        }
    }

    // Call this to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth} / {maxHealth}";

        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth; // Slider value goes from 0 to 1
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    // Stops shooting for all enemies
    void StopEnemyShooting()
    {
        foreach (var enemyGunManager in enemyGunManagers)
        {
            if (enemyGunManager is EnemyGunShootManager enemyGunShoot)
            {
                enemyGunShoot.StopShooting();
            }
            else if (enemyGunManager is TankGunShootManager tankGunShoot)
            {
                tankGunShoot.StopShooting();
            }
        }
    }
}
