using UnityEngine;

public class TargetBehaviorTestRotation : MonoBehaviour
{
    public float maxSpeed = 1000f;
    public float accelerationTime = 5f;
    public float decelerationTime = 5f;
    public float repeatDelay = 2f;

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float rotationSpeed = 0f;

        if (elapsedTime <= accelerationTime)
        {
            // Accelerate from 0 to max speed
            rotationSpeed = Mathf.Lerp(0f, maxSpeed,elapsedTime / accelerationTime);
        }
        else if (elapsedTime <= accelerationTime + decelerationTime)
        {
            // Decelerate from max speed to 0
            float t = (elapsedTime - accelerationTime) / decelerationTime;

            rotationSpeed = Mathf.Lerp(maxSpeed, 0f, t);
        }
        else if (elapsedTime >= accelerationTime + decelerationTime + repeatDelay)
        {
            // Restart the cycle
            elapsedTime = 0f;
        }

        // Rotate around Z axis
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
