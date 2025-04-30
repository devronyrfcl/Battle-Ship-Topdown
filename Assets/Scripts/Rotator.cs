using UnityEngine;
using DG.Tweening;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 45f; // Degrees per second

    private void Start()
    {
        // Calculate duration based on 360 degrees per full rotation
        float duration = 360f / rotationSpeed;

        // Infinite Y-axis rotation using DOTween
        transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1);
    }
}
