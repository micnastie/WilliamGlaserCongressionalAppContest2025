using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjScript : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 startingLocation = Vector3.zero; // teleport here on Start

    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // degrees per second
    public Vector3 rotationOffset = Vector3.zero;         // start offset

    [Header("Floating Settings")]
    public float floatAmplitude = 0.5f; // how far it moves up/down
    public float floatFrequency = 1f;   // speed of the bobbing motion

    [Header("Player Detection")]
    public Transform player;
    public float triggerDistance = 1f;

    private Vector3 startPos;

    
    void Start()
    {
        // If a starting location is set, teleport there
        if (startingLocation != Vector3.zero)
        {
            transform.position = startingLocation;
        }

        // Save starting position + apply offset rotation
        startPos = transform.position;
        transform.rotation = Quaternion.Euler(rotationOffset);

        // Auto-assign player if not set in Inspector
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Float up/down
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Player detection
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= triggerDistance)
            {
                GameObject.Find("QuestManager").GetComponent<QuestManagerScript>().endQuest();
                Debug.Log(gameObject.name + " got picked up!");
                Destroy(gameObject);
            }
        }
    }
}
