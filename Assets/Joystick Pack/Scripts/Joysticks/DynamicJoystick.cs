using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
    [SerializeField] private float moveThreshold = 1;

    [Header("Focus Point UIs")]
    public GameObject upperLeftFocus;
    public GameObject upperRightFocus;
    public GameObject bottomLeftFocus;
    public GameObject bottomRightFocus;

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        background.gameObject.SetActive(false);

        DeactivateAllFocusPoints();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        DeactivateAllFocusPoints();  // Deactivate focus points when releasing the joystick
        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }

        UpdateFocusPoints(normalised); // Update focus points based on input
        base.HandleInput(magnitude, normalised, radius, cam);
    }

    private void UpdateFocusPoints(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            DeactivateAllFocusPoints();
            return;
        }

        // Decide focus point based on the joystick direction
        if (direction.x > 0 && direction.y > 0) // Top Right
        {
            ActivateFocusPoint(upperRightFocus);
        }
        else if (direction.x < 0 && direction.y > 0) // Top Left
        {
            ActivateFocusPoint(upperLeftFocus);
        }
        else if (direction.x < 0 && direction.y < 0) // Bottom Left
        {
            ActivateFocusPoint(bottomLeftFocus);
        }
        else if (direction.x > 0 && direction.y < 0) // Bottom Right
        {
            ActivateFocusPoint(bottomRightFocus);
        }
    }

    private void ActivateFocusPoint(GameObject focusPoint)
    {
        DeactivateAllFocusPoints();
        if (focusPoint != null)
        {
            focusPoint.SetActive(true);
        }
    }

    private void DeactivateAllFocusPoints()
    {
        if (upperLeftFocus != null) upperLeftFocus.SetActive(false);
        if (upperRightFocus != null) upperRightFocus.SetActive(false);
        if (bottomLeftFocus != null) bottomLeftFocus.SetActive(false);
        if (bottomRightFocus != null) bottomRightFocus.SetActive(false);
    }
}
