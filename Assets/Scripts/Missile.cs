using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 3f;
    public float arcHeight = 5f;
    public GameObject[] explosionEffects;
    public float targetSearchRadius = 60f;

    private Transform target;
    private Vector3 startPoint;
    private Vector3 controlPoint;
    private float travelProgress = 0f;

    void Start()
    {
        FindNearestEnemy();

        if (target != null)
        {
            startPoint = transform.position;
            Vector3 midPoint = (startPoint + target.position) / 2;
            controlPoint = new Vector3(midPoint.x, midPoint.y + arcHeight, midPoint.z);
        }
        else
        {
            Debug.LogWarning("No enemy within range for missile.");
            Destroy(gameObject, 0.5f);
        }
    }

    void Update()
    {
        if (target == null) return;

        travelProgress += Time.deltaTime * speed;

        Vector3 a = Vector3.Lerp(startPoint, controlPoint, travelProgress);
        Vector3 b = Vector3.Lerp(controlPoint, target.position, travelProgress);
        transform.position = Vector3.Lerp(a, b, travelProgress);
        transform.LookAt(b);

        if (travelProgress >= 1f)
        {
            OnHit();
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = targetSearchRadius;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
    }

    void OnHit()
    {
        if (explosionEffects != null && explosionEffects.Length > 0)
        {
            GameObject randomEffect = explosionEffects[Random.Range(0, explosionEffects.Length)];
            Instantiate(randomEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
        }

        Destroy(gameObject, 0.01f);
    }

    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Vector3 midPoint = (transform.position + target.position) / 2;
            midPoint.y += arcHeight;
            Gizmos.DrawLine(transform.position, midPoint);
            Gizmos.DrawLine(midPoint, target.position);
        }
    }
}
