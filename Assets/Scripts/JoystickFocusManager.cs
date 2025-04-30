using UnityEngine;

public class JoystickFocusManager : MonoBehaviour
{
    [Header("References")]
    public DynamicJoystick dynamicJoystick;

    [Header("Focus Points")]
    public GameObject upperLeftFocus;
    public GameObject upperRightFocus;
    public GameObject bottomLeftFocus;
    public GameObject bottomRightFocus;

    [Header("Settings")]
    public float deadZone = 0.2f; // Ignore small movements

    void Update()
    {
        Vector2 inputDirection = new Vector2(dynamicJoystick.Horizontal, dynamicJoystick.Vertical);

        if (inputDirection.magnitude < deadZone)
        {
            DeactivateAllFocusPoints();
            return;
        }

        if (inputDirection.y > 0) // Up
        {
            if (inputDirection.x < 0)
            {
                ActivateFocus(upperLeftFocus);
            }
            else
            {
                ActivateFocus(upperRightFocus);
            }
        }
        else // Down
        {
            if (inputDirection.x < 0)
            {
                ActivateFocus(bottomLeftFocus);
            }
            else
            {
                ActivateFocus(bottomRightFocus);
            }
        }
    }

    void ActivateFocus(GameObject focusPoint)
    {
        upperLeftFocus.SetActive(focusPoint == upperLeftFocus);
        upperRightFocus.SetActive(focusPoint == upperRightFocus);
        bottomLeftFocus.SetActive(focusPoint == bottomLeftFocus);
        bottomRightFocus.SetActive(focusPoint == bottomRightFocus);
    }

    void DeactivateAllFocusPoints()
    {
        upperLeftFocus.SetActive(false);
        upperRightFocus.SetActive(false);
        bottomLeftFocus.SetActive(false);
        bottomRightFocus.SetActive(false);
    }
}
