using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeAndDestroy : MonoBehaviour
{
    [Header("Timing Settings")]
    public float solidTime = 2f;   // How long it stays fully visible
    public float fadeTime = 1f;    // How long it takes to fade out

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f; // Start fully visible
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        // Stay solid for X seconds
        yield return new WaitForSeconds(solidTime);

        // Fade over Y seconds
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Destroy itself once invisible
        Destroy(gameObject);
    }
}
