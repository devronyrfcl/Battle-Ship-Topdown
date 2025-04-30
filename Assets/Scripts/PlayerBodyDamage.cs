using UnityEngine;
using MoreMountains.Feedbacks; // Make sure you have this using for Feel
using DG.Tweening;

public class PlayerBodyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 5;
    public HealthManager healthManager;

    public Transform mainPropeller;
    public Transform backPropeller;
    public float mainpropellerSpeed = 500f;
    public float minipropellerSpeed = 1000f;

    [Header("Audio")]
    public AudioSource hitAudioSource;
    public AudioClip[] hitSounds;

    private MMF_Player feelPlayer; // <- Added

    public ShootingManager shootingManager;

    void Start()
    {
        // Find and assign Feel MMF_Player
        GameObject feelObject = GameObject.Find("Feel_MMF_Player");
        if (feelObject != null)
        {
            feelPlayer = feelObject.GetComponent<MMF_Player>();
        }
        else
        {
            Debug.LogWarning("Feel_MMF_Player not found in the scene.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount);
                Debug.Log("Player hit by EnemyBullet! Took damage: " + damageAmount);
            }

            PlayRandomHitSound();

            if (feelPlayer != null)
            {
                feelPlayer.PlayFeedbacks(); // <- Play Feel feedback
            }
        }
        else if (other.CompareTag("Health_Collectable"))
        {
            if (healthManager != null)
            {
                healthManager.Heal(50);
                Debug.Log("Picked up Health Collectable! Health +30");
            }
            Destroy(other.gameObject); // Remove the collectable after picking up
        }
        else if (other.CompareTag("Bullet_Collectable"))
        {
            if (shootingManager != null)
            {
                shootingManager.AddBullets(200);
                Debug.Log("Picked up Bullet Collectable! Bullets +100");
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Missile_Collectable"))
        {
            if (shootingManager != null)
            {
                shootingManager.AddMissiles(20);
                Debug.Log("Picked up Missile Collectable! Missiles +5");
            }
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        // Rotate propellers
        if (mainPropeller != null)
            mainPropeller.Rotate(Vector3.up, mainpropellerSpeed * Time.deltaTime);
        if (backPropeller != null)
            backPropeller.Rotate(Vector3.right, minipropellerSpeed * Time.deltaTime);
    }

    void PlayRandomHitSound()
    {
        if (hitAudioSource != null && hitSounds != null && hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            hitAudioSource.clip = hitSounds[randomIndex];
            hitAudioSource.Play();
        }
    }
}
