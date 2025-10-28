using UnityEngine;
using System.Collections;

public class PlaneFolder : MonoBehaviour
{
    [Header("References")]
    public Transform hinge;  // Empty pivot
    public Transform plane;  // The plane mesh itself

    [Header("Animation Settings")]
    public float foldDuration = 1.2f;                     // Total animation time
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Rotation over time
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);    // Scale multiplier over time
    public Vector3 maxScaleMultiplier = new Vector3(1.2f, 1.2f, 1f);             // Max stretching on X, Y, Z

    [Header("Fold Settings")]
    public Vector3 foldEuler = new Vector3(90, 0, 0); // Snap rotation before folding back

    private Quaternion originalRotation;
    private Vector3 originalScale;
    private bool isFolding = false;

    void Awake()
    {
        if (plane == null) plane = transform;

        // Store original rotation & scale
        originalRotation = hinge.rotation;
        originalScale = plane.localScale;
        FoldUp();
    }

    /// <summary>
    /// Fold the plane up from foldEuler back to original rotation
    /// </summary>
    public void FoldUp()
    {
        if (!isFolding)
            StartCoroutine(FoldRoutine());
    }

    private IEnumerator FoldRoutine()
    {
        isFolding = true;

        // Snap hinge to folded orientation
        hinge.rotation = Quaternion.Euler(foldEuler);
        plane.localScale = originalScale;

        Quaternion startRot = hinge.rotation;
        Quaternion endRot = originalRotation;

        float timer = 0f;
        while (timer < foldDuration)
        {
            timer += Time.deltaTime;
            float t = timer / foldDuration;

            // Rotation based on curve
            float rotT = rotationCurve.Evaluate(t);
            hinge.rotation = Quaternion.Slerp(startRot, endRot, rotT);

            // Scale based on curve
            float scaleT = scaleCurve.Evaluate(t);
            plane.localScale = new Vector3(
                originalScale.x * (1 + (maxScaleMultiplier.x - 1) * scaleT),
                originalScale.y * (1 + (maxScaleMultiplier.y - 1) * scaleT),
                originalScale.z * (1 + (maxScaleMultiplier.z - 1) * scaleT)
            );

            yield return null;
        }

        // Snap to final values
        hinge.rotation = endRot;
        plane.localScale = originalScale;

        isFolding = false;
    }
}
