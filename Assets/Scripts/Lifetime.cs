using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
