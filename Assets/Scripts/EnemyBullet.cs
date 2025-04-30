using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnHitPlayer();
        }
    }

    void OnHitPlayer()
    {
        Debug.Log("EnemyBullet hit the Player!");
        Destroy(gameObject, 0.01f);
    }
}
