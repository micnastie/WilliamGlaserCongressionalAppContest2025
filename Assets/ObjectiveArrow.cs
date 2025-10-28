using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    [Header("References")]
    public Transform objective;        // The target object to highlight
    public Camera cam;                 // Main camera
    public RectTransform arrowUI;      // UI arrow element (in Screen Space Canvas)

    [Header("Settings")]
    public float heightOffset = 2f;    // How high above the objective the arrow floats

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (objective == null || cam == null || arrowUI == null) return;

        // World position just above the objective
        Vector3 worldPos = objective.position + Vector3.up * heightOffset;

        // Convert world position → screen space
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // Place arrow on screen
        arrowUI.position = screenPos;

        // Optionally hide if objective is behind the camera
        arrowUI.gameObject.SetActive(screenPos.z > 0);
    }
}
