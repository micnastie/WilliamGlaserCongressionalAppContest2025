using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewManager : MonoBehaviour
{
    [SerializeField] private DialologManager dialogManager;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Image background;
    [SerializeField] private Image grid;
    [SerializeField] private GameObject reviewButton;
    private bool isReviewing = false;

    private void Update()
    {
       
    }
    // Button hook
    public void StartReviewFromButton()
    {
        if (!isReviewing)
        {
            StartCoroutine(EndOfDayReview());
        }
    }

    private IEnumerator EndOfDayReview()
    {
        dialogManager.isReviewing = true;
        isReviewing = true;
        if (dialogManager == null)
        {
            Debug.LogError("[ReviewManager] No DialogManager set!");
            yield break;
        }

        winPanel.gameObject.SetActive(false);
       

        grid.color = ColorUtility.TryParseHtmlString("#FF55E1FF", out Color c) ? c : Color.white;
        background.color = ColorUtility.TryParseHtmlString("#E200FF80", out c) ? c : Color.white;

        // Process until all wrongConvos are gone
        while (dialogManager.wrongConvos.Count > 0)
        {
            Conversation convo = dialogManager.wrongConvos[0];
            dialogManager.wrongConvos.RemoveAt(0); // remove before playing

            Debug.Log(convo + " STARTING CONVO REVIEW");
            yield return StartCoroutine(dialogManager.loadConvo(convo));
            Debug.Log("A REVIEW FINISHED!");
            dialogManager.au.PlaySFX("jingles_SAX16");
        }

        isReviewing = false;
        Debug.Log("[ReviewManager] All reviews finished!");
        StartCoroutine(dialogManager.fadeToMainMenu());
    }
}
