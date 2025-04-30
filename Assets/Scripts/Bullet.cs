using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move in global X direction
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            OnHitEnemy();
        }
    }

    void OnHitEnemy()
    {
        Debug.Log("Bullet hit the Enemy!");
        Destroy(gameObject, 0.005f);
    }
}
