using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // degrees per second

    [Header("Floating Settings")]
    public float floatAmplitude = 0.5f; // how far it moves up and down
    public float floatFrequency = 1f;   // speed of the up/down motion

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // remember starting position
    }

    void Update()
    {
        // Rotate
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Float up and down using sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
