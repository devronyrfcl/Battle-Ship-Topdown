using UnityEngine;
using UnityEngine.UI;

public class TankScript : MonoBehaviour
{
    public Transform firePoint;
    public GameObject enemyBulletPrefab;

    [Header("Fire Settings")]
    public float fireRate = 3f; // Tank fires every 3 seconds
    public int shotsBeforeReload = 5;
    public float reloadTime = 2f;
    public float shootingRange = 30f;

    [Header("Health Settings")]
    public Image healthImage;
    public int maxHealth = 20;

    [Header("Hit Particles")]
    public GameObject bulletHitParticlePrefab;
    public GameObject missileHitParticlePrefab;

    [Header("Audio")]
    public AudioSource deathSound;
    public AudioSource shootSound;

    private int currentHealth;
    private float fireTimer = 0f;
    private int shotsFired = 0;
    private bool isReloading = false;
    private bool isDead = false;

    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isReloading || isDead) return;

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

            if (shootSound != null)
                shootSound.Play();
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
            Debug.Log("Tank Hit by Bullet!");
            SpawnParticle(bulletHitParticlePrefab, true);
            TakeDamage(1);
        }

        if (other.CompareTag("Missile"))
        {
            Debug.Log("Tank Hit by Missile!");
            SpawnParticle(missileHitParticlePrefab, false);
            TakeDamage(5);
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Tank Destroyed!");
            if (deathSound != null)
                deathSound.Play();
        }
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

            Instantiate(particlePrefab, spawnPos, rotation);
        }
    }
}
