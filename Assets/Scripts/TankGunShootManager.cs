using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Feedbacks; // Make sure you have this using for Feel

public class TankGunShootManager : MonoBehaviour
{
    [Header("Tank Parts")]
    public Transform turretTransform;
    public Transform barrelTransform;
    public Transform firePoint;

    [Header("Shooting")]
    public GameObject tankBulletPrefab;
    public float fireRate = 1f;
    public int shotsBeforeReload = 5;
    public float reloadTime = 2f;
    public float shootingRange = 25f;

    [Header("Health")]
    public Image healthImage;
    private int maxHealth = 20;

    [Header("Audio")]
    public AudioSource deathSound;
    public AudioSource hitSoundSource;
    public AudioClip[] bulletHitClips;

    [Header("Destroyed Version")]
    public GameObject destroyedTankPrefab;   // <<< Drag your destroyed/broken tank prefab here!

    private Transform player;
    private int currentHealth;
    private float fireTimer = 0f;
    private int shotsFired = 0;
    private bool isReloading = false;
    private bool isDead = false;
    private bool canShoot = true; // To control shooting functionality

    private MMF_Player feelPlayer; // <- Added

    private StatsCounter statsCounter;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        StartCoroutine(FindPlayerWithDelay());

        statsCounter = FindObjectOfType<StatsCounter>(); // Find it in the scene

        // Find and assign Feel MMF_Player
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
        if (player == null || isReloading || isDead || !canShoot) return; // Stop update if canShoot is false

        RotateTurretAndBarrel();

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

    void RotateTurretAndBarrel()
    {
        // Turret rotation (only Y axis)
        Vector3 turretDirection = player.position - turretTransform.position;
        turretDirection.y = 0f;

        if (turretDirection.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(turretDirection);
            Vector3 targetEuler = lookRotation.eulerAngles;

            Vector3 currentEuler = turretTransform.rotation.eulerAngles;
            currentEuler.y = Mathf.LerpAngle(currentEuler.y, targetEuler.y + 180f, Time.deltaTime * 5f);
            turretTransform.rotation = Quaternion.Euler(currentEuler);
        }

        // Barrel rotation (only X axis)
        Vector3 barrelDirection = player.position - barrelTransform.position;
        Quaternion barrelLook = Quaternion.LookRotation(barrelDirection);
        float xAngle = barrelLook.eulerAngles.x;
        if (xAngle > 180f) xAngle -= 360f;
        xAngle = Mathf.Clamp(xAngle, 0f, 25f);
        barrelTransform.localRotation = Quaternion.Euler(xAngle, 0f, 0f);
    }

    void ShootAtPlayer()
    {
        if (tankBulletPrefab != null && firePoint != null)
        {
            Vector3 shootDir = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(tankBulletPrefab, firePoint.position, Quaternion.LookRotation(shootDir));

            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.SetDirection(shootDir);
            }
        }
    }

    void FinishReloading()
    {
        shotsFired = 0;
        isReloading = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
            PlayRandomHitSound();
            TakeDamage(1);
        }

        if (other.CompareTag("Missile"))
        {
            PlayRandomHitSound();
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

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameObject.tag = "Untagged";

            if (statsCounter != null)
            {
                statsCounter.AddDeathCount(); // Call to add death count
                statsCounter.AddCoins(10);     // Maybe reward 10 coins for killing an enemy
            }

            // Stop shooting when dead
            StopShooting();

            if (deathSound != null)
                deathSound.Play();

            if (destroyedTankPrefab != null)
            {
                Instantiate(destroyedTankPrefab, transform.position, transform.rotation);
            }

            Destroy(gameObject, 0.5f); // Delay destroy to allow death sound to play
        }
    }

    public void StopShooting()
    {
        canShoot = false; // This will stop the shooting mechanism
    }

    void PlayRandomHitSound()
    {
        if (hitSoundSource != null && bulletHitClips.Length > 0)
        {
            AudioClip clip = bulletHitClips[Random.Range(0, bulletHitClips.Length)];
            hitSoundSource.PlayOneShot(clip);
        }
    }

    void UpdateHealthUI()
    {
        if (healthImage != null)
            healthImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
