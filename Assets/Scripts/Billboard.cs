using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Make the object face the camera
            transform.LookAt(mainCamera.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Optional: lock X and Z
        }
    }
}
