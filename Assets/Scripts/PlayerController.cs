using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float slideSpeed = 8f;
    public float slideLimit = 15f;
    public float forwardSlideSpeed = 8f;
    public float forwardSlideLimit = 20f;

    [Header("Helicopter Parts")]
    public Transform[] playerBodies; // Array of skins
    public Transform firePointParent; // New Fire Point Parent!
    public Transform ParticlesParent;

    [Header("Joystick Input")]
    public Joystick joystick;

    [Header("Helicopter Sound")]
    public AudioSource helicopterAudioSource;
    public float engineVolume = 0.5f;
    public float enginePitch = 1.0f;

    private float slideDirectionX = 0f;
    private float slideDirectionZ = 0f;

    private bool isDead = false;
    public HealthManager healthManager;

    [Header("Damage Particles")]
    public GameObject[] damageParticles; // Array of damage particle effects

    [Header("UI Elements")]
    public GameObject gameOverUI; // Reference to the Game Over UI
    public StatsCounter statsCounter;
    public Button missileButton;
    public Button bulletButton;

    public ShootingManager shootingManager;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        if (helicopterAudioSource != null)
        {
            helicopterAudioSource.loop = true;
            helicopterAudioSource.volume = engineVolume;
            helicopterAudioSource.pitch = enginePitch;
            helicopterAudioSource.Play();
        }

        //ShootingManager shootingManager = GetComponent<ShootingManager>();
    }

    void Update()
    {
        if (healthManager != null && healthManager.currentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnPlayerDead();
        }

        if (isDead) return;

        // Forward movement
        transform.position += Vector3.right * forwardSpeed * Time.deltaTime;

        // Joystick input
        slideDirectionX = joystick.Horizontal;
        slideDirectionZ = joystick.Vertical;

        // Find the active player body
        Transform activeBody = GetActivePlayerBody();
        if (activeBody != null)
        {
            Vector3 localPos = activeBody.localPosition;
            localPos.x += slideDirectionX * slideSpeed * Time.deltaTime;
            localPos.z += slideDirectionZ * forwardSlideSpeed * Time.deltaTime;

            localPos.x = Mathf.Clamp(localPos.x, -slideLimit, slideLimit);
            localPos.z = Mathf.Clamp(localPos.z, -2f, 20f);

            activeBody.localPosition = localPos;

            Quaternion targetRot = Quaternion.Euler(
                -15f * slideDirectionZ,
                -90f,
                25f * slideDirectionX
            );
            activeBody.DORotateQuaternion(targetRot, 0.25f);

            // Move firePointParent to match the active body's local position
            if (firePointParent != null)
            {
                firePointParent.localPosition = localPos;
            }

            // Sync ParticlesParent position with the Y offset (Y + 3.42f)
            if (ParticlesParent != null)
            {
                Vector3 particlesPos = localPos;
                particlesPos.y += 3.42f; // Add the Y offset
                ParticlesParent.localPosition = particlesPos;
            }
        }

        // Trigger damage particles based on health
        TriggerDamageParticles();
    }

    void OnPlayerDead()
    {
        // Set propeller speeds to 0 smoothly
        foreach (Transform body in playerBodies)
        {
            PlayerBodyDamage playerBodyDamage = body.GetComponent<PlayerBodyDamage>();
            if (playerBodyDamage != null)
            {
                // Smoothly reduce the propeller speeds to 0
                DOTween.To(() => playerBodyDamage.mainpropellerSpeed, x => playerBodyDamage.mainpropellerSpeed = x, 0f, 2f);
                DOTween.To(() => playerBodyDamage.minipropellerSpeed, x => playerBodyDamage.minipropellerSpeed = x, 0f, 2f);
            }

            // Disable the MeshCollider's trigger and set gravity on the Rigidbody
            MeshCollider meshCollider = body.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.isTrigger = false; // Disable the trigger
            }

            Rigidbody rb = body.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true; // Enable gravity for realistic falling
            }

            // Unset the tag of the playerBody (remove any assigned tag)
            body.tag = "Untagged"; // Unset the tag (remove any tag)

            // Change the material color to black instantly
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = Color.black;
            }
        }

        // Disable the ShootingManager script
        ShootingManager shootingManager = GetComponent<ShootingManager>();
        if (shootingManager != null)
        {
            shootingManager.enabled = false; // Disable the script
        }

        // Smoothly fade out the helicopter sound
        if (helicopterAudioSource != null)
        {
            helicopterAudioSource.DOFade(0f, 2f).OnKill(() => helicopterAudioSource.Stop());
        }

        // Activate the Game Over UI
        StartCoroutine(EnablegameOverUIAfterDelay());


        if (statsCounter != null)
        {
            statsCounter.GameOver();
        }

        // Stop all movement
        forwardSpeed = 0f;
        slideSpeed = 0f;
        forwardSlideSpeed = 0f;

        // Find the active player body and set the ParticlesParent as its child
        Transform activeBody = GetActivePlayerBody();
        if (activeBody != null)
        {
            // Set the ParticlesParent as a child of the active player body
            ParticlesParent.SetParent(activeBody);

            // Set the position of ParticlesParent relative to the active player body
            Vector3 particlesPos = activeBody.localPosition;
            particlesPos.y += 3.42f; // Add the Y offset to match the position
            ParticlesParent.localPosition = particlesPos;
        }

        

        if (missileButton != null)
        {
            missileButton.interactable = false; // Disable missile button
        }

        if (bulletButton != null)
        {
            bulletButton.interactable = false; // Disable bullet button
        }

        Debug.Log("Player is dead. Enemy is killed.");
    }


    IEnumerator EnablegameOverUIAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(3);

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    private Transform GetActivePlayerBody()
    {
        foreach (Transform body in playerBodies)
        {
            if (body != null && body.gameObject.activeSelf)
            {
                return body;
            }
        }
        return null;
    }

    private void TriggerDamageParticles()
    {
        if (healthManager != null)
        {
            // Check if health is at full or only slightly reduced (maxHealth - 10)
            if (healthManager.currentHealth == healthManager.maxHealth || healthManager.currentHealth == healthManager.maxHealth - 10)
            {
                // If health is full or only slightly reduced, no particles should be activated.
                DeactivateAllParticles();
                return;
            }

            // Calculate the number of particles to activate based on the health percentage
            float healthPercentage = (float)healthManager.currentHealth / healthManager.maxHealth;
            int numActivatedParticles = Mathf.FloorToInt((1 - healthPercentage) * damageParticles.Length);

            // Activate the corresponding number of damage particles
            for (int i = 0; i < damageParticles.Length; i++)
            {
                if (i <= numActivatedParticles)
                {
                    if (!damageParticles[i].activeSelf)
                    {
                        damageParticles[i].SetActive(true);
                    }
                }
                else
                {
                    if (damageParticles[i].activeSelf)
                    {
                        damageParticles[i].SetActive(false);
                    }
                }
            }
        }
    }

    private void DeactivateAllParticles()
    {
        foreach (var particle in damageParticles)
        {
            if (particle.activeSelf)
            {
                particle.SetActive(false);
            }
        }
    }
}
