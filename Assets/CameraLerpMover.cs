using UnityEngine;
using System.Collections;

public class CameraLerpMover : MonoBehaviour
{
    [Header("Lerp Settings")]
    public float moveDuration = 1.0f;      // How long the camera takes to reach the target
    public AnimationCurve easeCurve;       // Optional easing curve

    private Coroutine moveCoroutine;

    /// <summary>
    /// Smoothly move the camera to a target Vector3 position.
    /// </summary>
    public void MoveTo(Vector3 targetPos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(LerpToPosition(targetPos));
    }

    private IEnumerator LerpToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            if (easeCurve != null)
                t = easeCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        transform.position = targetPos;
        moveCoroutine = null;
    }
}
