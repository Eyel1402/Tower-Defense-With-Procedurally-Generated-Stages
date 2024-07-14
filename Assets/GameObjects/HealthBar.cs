using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private Transform target;
    private Camera mainCamera;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, 0); // Default offset

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        target = transform.parent != null ? transform.parent : transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (target != null && mainCamera != null)
        {
            transform.position = target.position + offset;
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void UpdateHealthBar(int currentValue, int maxValue)
    {
        if (slider != null)
        {
            slider.value = (float)currentValue / maxValue;
        }
        else
        {
            Debug.LogError("Slider component not found in children of the HealthBar GameObject.");
        }
    }
}
