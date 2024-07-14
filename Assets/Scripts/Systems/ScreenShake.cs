using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;

    private Transform cameraTransform;
    private Vector3 initialPosition;
    private float shakeTimeRemaining;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        initialPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            if (!CameraControl.isMoving) // Check if the camera is not moving
            {
                cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
                shakeTimeRemaining -= Time.deltaTime;
            }
        }
        else
        {
            shakeTimeRemaining = 0f;
            cameraTransform.localPosition = initialPosition;
        }
    }

    public void TriggerShake()
    {
        shakeTimeRemaining = shakeDuration;
    }
}
