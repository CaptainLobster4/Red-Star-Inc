using UnityEngine;
using System.Collections;

public class TTargetBehaviorTestMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 3f;
    public float movementDuration = 3f;
    public float pauseDuration = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        // Store the original center position
        startPosition = transform.position;

        // Start the repeating movement
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (true)
        {
            // Center to Left
            yield return MoveTo(
                startPosition.x - moveDistance,
                movementDuration / 3f
            );

            // Left to Right
            yield return MoveTo(
                startPosition.x + moveDistance,
                movementDuration / 3f
            );

            // Right to Center
            yield return MoveTo(
                startPosition.x,
                movementDuration / 3f
            );

            // Pause before repeating
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    IEnumerator MoveTo(float targetX, float duration)
    {
        Vector3 initialPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            targetX,
            startPosition.y,
            startPosition.z
        );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                initialPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        // Ensure exact final position
        transform.position = targetPosition;
    }
}