using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRotated : MonoBehaviour
{
     // Speed of rotation around the z-axis
    public float rotationSpeed = 100f;
    // Update is called once per frame
    void Update()
    {
        // Calculate the rotation amount based on time and speed
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Rotate the GameObject around its local z-axis
        transform.Rotate(0, 0, rotationAmount);
    }
}
