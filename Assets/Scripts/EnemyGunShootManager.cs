using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Feedbacks; // Make sure you have this using for Feel

public class EnemyGunShootManager : MonoBehaviour
{
    public Transform firePoint;
    public GameObject enemyBulletPrefab;

    [Header("Fire Settings")]
    public float fireRate = 1f;
    public int shotsBeforeReload = 5;
    public float reloadTime = 2f;
    public float shootingRange = 25f;

    [Header("Health Settings")]
    public Image healthImage;
    private int maxHealth = 10;

    [Header("Hit Particles")]
    public GameObject bulletHitParticlePrefab;
    public GameObject missileHitParticlePrefab;

    [Header("Audio")]
    public AudioSource deathSound;

    [Header("Animation and Gun")]
    public Animator animator;
    public GameObject gunObject;

    private int currentHealth;
    private float fireTimer = 0f;
    private int shotsFired = 0;
    private bool isReloading = false;
    private bool isDead = false;
    private bool canShoot = true; // New variable to control shooting

    private MMF_Player feelPlayer; // <- Added

    private StatsCounter statsCounter;

    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        StartCoroutine(FindPlayerWithDelay());

        statsCounter = FindObjectOfType<StatsCounter>(); // Find it in the scene

        GameObject feelObject = GameObject.Find("Feel_MMF_Enemy");
        if (feelObject != null)
        {
            feelPlayer = feelObject.GetComponent<MMF_Player>();
        }
        else
        {
            Debug.LogWarning("Feel_MMF_Player not found in the scene.");
        }
    }

    IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Delay for 1.5 seconds
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isReloading || isDead || !canShoot) return;

        RotateTowardsPlayer();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > shootingRange) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootAtPlayer();
            shotsFired++;

            if (shotsFired >= shotsBeforeReload)
            {
                isReloading = true;
                Invoke(nameof(FinishReloading), reloadTime);
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    void ShootAtPlayer()
    {
        if (enemyBulletPrefab != null && firePoint != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.SetDirection(direction);
            }
        }
    }

    void FinishReloading()
    {
        shotsFired = 0;
        isReloading = false;
    }

    void UpdateHealthUI()
    {
        if (healthImage != null)
            healthImage.fillAmount = (float)currentHealth / maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
            Debug.Log("Enemy Hit by Bullet!");
            SpawnParticle(bulletHitParticlePrefab, true);
            TakeDamage(1);
        }

        if (other.CompareTag("Missile"))
        {
            Debug.Log("Enemy Hit by Missile!");
            SpawnParticle(missileHitParticlePrefab, false);
            TakeDamage(5);

            if (feelPlayer != null)
            {
                feelPlayer.PlayFeedbacks(); // <- Play Feel feedback
            }
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();

            if (statsCounter != null)
            {
                statsCounter.AddDeathCount(); // Call to add death count
                statsCounter.AddCoins(10);     // Maybe reward 10 coins for killing an enemy
            }
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy Dead!");
        gameObject.tag = "Untagged";

        if (deathSound != null)
            deathSound.Play();

        if (animator != null)
            animator.SetTrigger("Death");

        if (gunObject != null)
            gunObject.SetActive(false);


    }

    void SpawnParticle(GameObject particlePrefab, bool isBullet = false)
    {
        if (particlePrefab != null)
        {
            Vector3 spawnPos;
            Quaternion rotation = Quaternion.identity;

            if (isBullet)
            {
                spawnPos = transform.position + Vector3.right * -1f + Vector3.up * 1.5f;
                rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else
            {
                spawnPos = transform.position + Vector3.up * 3f;
            }

            GameObject particle = Instantiate(particlePrefab, spawnPos, rotation);

            if (!isBullet && particle != null)
            {
                Destroy(particle, 3f);
            }
        }
    }

    // Stop shooting when health is 0
    public void StopShooting()
    {
        canShoot = false;
    }
}
