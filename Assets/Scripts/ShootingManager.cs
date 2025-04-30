using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;  // <-- Make sure to import DOTween

[System.Serializable]
public class Gun
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.1f;

    [HideInInspector]
    public float fireTimer = 0f;
}

[System.Serializable]
public class MissileLauncher
{
    public Transform firePoint;
    public GameObject missilePrefab;
}

public class ShootingManager : MonoBehaviour
{
    [Header("Guns Setup")]
    public Gun[] guns;
    public int bulletCount = 1000;
    public int bulletsPerMagazine = 30;
    public float bulletReloadTime = 2f;

    private int bulletsInMagazine;
    private bool isReloadingBullets = false;

    [Header("Missile Setup")]
    public MissileLauncher[] missiles;
    public int missileCount = 50;
    public float missileReloadTime = 3f;
    private bool isReloadingMissile = false;
    private bool missileLoaded = true;

    [Header("UI Elements")]
    public TMP_Text bulletText;
    public TMP_Text missileText;
    public TMP_Text noTargetWarningText;
    public TMP_Text noBulletWarningText;
    public TMP_Text missilesFinishedWarningText;
    public GameObject warningFrameUI;

    [Header("Sliders")]
    public Slider bulletSlider;
    public Slider missileSlider;

    [Header("Audio")]
    public AudioSource WarningSound;
    public AudioSource NoBullets;

    private bool isFiringGuns = false;

    void Start()
    {
        if (noTargetWarningText != null) noTargetWarningText.gameObject.SetActive(false);
        if (noBulletWarningText != null) noBulletWarningText.gameObject.SetActive(false);
        if (missilesFinishedWarningText != null) missilesFinishedWarningText.gameObject.SetActive(false);
        if (warningFrameUI != null) warningFrameUI.SetActive(false);

        bulletsInMagazine = bulletsPerMagazine;
        UpdateUI();

        if (bulletSlider != null) bulletSlider.value = 1;
        if (missileSlider != null) missileSlider.value = 1;
    }

    void Update()
    {
        if (isFiringGuns && !isReloadingBullets)
        {
            if (bulletCount <= 0 || bulletsInMagazine <= 0)
            {
                if (bulletsInMagazine <= 0)
                    StartCoroutine(ReloadBullets());

                if (bulletCount <= 0)
                {
                    if (!NoBullets.isPlaying)
                        NoBullets.Play();

                    if (noBulletWarningText != null)
                        StartCoroutine(BlinkNoBulletText());
                }

                return;
            }

            foreach (Gun gun in guns)
            {
                gun.fireTimer += Time.deltaTime;

                if (gun.fireTimer >= gun.fireRate)
                {
                    FireGun(gun);
                    gun.fireTimer = 0f;
                }
            }
        }
    }

    void FireGun(Gun gun)
    {
        if (gun.firePoint != null && gun.bulletPrefab != null && bulletCount > 0 && bulletsInMagazine > 0)
        {
            Instantiate(gun.bulletPrefab, gun.firePoint.position, gun.firePoint.rotation);
            bulletCount--;
            bulletsInMagazine--;
            UpdateUI();
            UpdateBulletSlider();
        }
    }

    public void FireMissile()
    {
        if (!missileLoaded || isReloadingMissile)
        {
            Debug.Log("Missile is reloading...");
            return;
        }

        if (missileCount <= 0)
        {
            if (WarningSound != null)
                WarningSound.Play();

            if (missilesFinishedWarningText != null)
                StartCoroutine(BlinkMissilesFinishedText());

            return;
        }

        bool enemyInRange = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= 60f)
            {
                enemyInRange = true;
                break;
            }
        }

        if (!enemyInRange)
        {
            Debug.Log("No enemies in range. Aborting missile fire.");
            if (noTargetWarningText != null)
                StartCoroutine(BlinkNoTargetText());

            if (WarningSound != null)
                WarningSound.Play();

            return;
        }

        foreach (MissileLauncher missile in missiles)
        {
            if (missile.firePoint != null && missile.missilePrefab != null)
            {
                Instantiate(missile.missilePrefab, missile.firePoint.position, missile.firePoint.rotation);
            }
        }

        missileCount -= 2;
        missileLoaded = false;
        UpdateUI();
        StartCoroutine(ReloadMissile());
    }

    IEnumerator ReloadBullets()
    {
        isReloadingBullets = true;

        if (bulletSlider != null && bulletCount > 0 && bulletsInMagazine <= 0)
        {
            float timer = 0f;
            while (timer < bulletReloadTime)
            {
                timer += Time.deltaTime;
                float sliderValue = timer / bulletReloadTime;
                bulletSlider.DOValue(sliderValue, 0.1f); // Smooth transition using DOTween
                yield return null;
            }
            bulletSlider.value = 0f;
        }

        // No reload when no bullets are available
        if (bulletCount <= 0)
        {
            bulletSlider.value = 0f;
        }

        bulletsInMagazine = Mathf.Min(bulletsPerMagazine, bulletCount);
        isReloadingBullets = false;
    }

    IEnumerator ReloadMissile()
    {
        isReloadingMissile = true;

        if (missileSlider != null && missileCount > 0)
        {
            float timer = 0f;
            while (timer < missileReloadTime)
            {
                timer += Time.deltaTime;
                float sliderValue = timer / missileReloadTime;
                missileSlider.DOValue(sliderValue, 0.1f); // Smooth transition using DOTween
                yield return null;
            }
            missileSlider.value = 0f;
        }

        if (missileCount <= 0)
        {
            missileSlider.value = 0f;
        }

        missileLoaded = true;
        isReloadingMissile = false;
    }

    void UpdateBulletSlider()
    {
        if (bulletSlider != null && bulletCount > 0 && bulletsInMagazine > 0)
        {
            float sliderValue = (float)bulletsInMagazine / bulletsPerMagazine;
            bulletSlider.DOValue(sliderValue, 0.1f); // Smooth transition using DOTween
        }
        else
        {
            bulletSlider.value = 0f;
        }
    }

    void UpdateMissileSlider()
    {
        if (missileSlider != null && missileCount > 0)
        {
            missileSlider.DOValue((float)missileCount / 50f, 0.1f); // Assuming max 50 missiles
        }
        else
        {
            missileSlider.value = 0f;
        }
    }

    IEnumerator BlinkNoTargetText()
    {
        yield return BlinkText(noTargetWarningText);
    }

    IEnumerator BlinkNoBulletText()
    {
        yield return BlinkText(noBulletWarningText);
    }

    IEnumerator BlinkMissilesFinishedText()
    {
        yield return BlinkText(missilesFinishedWarningText);
    }

    IEnumerator BlinkText(TMP_Text textElement)
    {
        if (textElement == null) yield break;

        textElement.gameObject.SetActive(true);

        if (warningFrameUI != null)
            warningFrameUI.SetActive(true);

        float blinkDuration = 2f;
        float timer = 0f;
        bool visible = true;

        while (timer < blinkDuration)
        {
            textElement.alpha = visible ? 1 : 0;
            visible = !visible;
            timer += 0.3f;
            yield return new WaitForSeconds(0.3f);
        }

        textElement.alpha = 0;
        textElement.gameObject.SetActive(false);

        if (warningFrameUI != null)
            warningFrameUI.SetActive(false);
    }

    public void StartFiringGuns() => isFiringGuns = true;

    public void StopFiringGuns() => isFiringGuns = false;

    public void AddBullets(int amount)
    {
        bulletCount += amount;
        if (bulletCount > 0)
        {
            UpdateUI();
            if (bulletSlider != null)
                bulletSlider.value = 1f; // Reset slider to 1 after adding bullets
        }
    }

    public void AddMissiles(int amount)
    {
        missileCount += amount;
        if (missileCount > 0)
        {
            UpdateUI();
            if (missileSlider != null)
                missileSlider.value = 1f; // Reset slider to 1 after adding missiles
        }
    }

    void UpdateUI()
    {
        if (bulletText != null)
            bulletText.text = $"Bullets: {bulletCount}";

        if (missileText != null)
            missileText.text = $"Missiles: {missileCount}";
    }

}
