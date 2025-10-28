using UnityEngine;
using TMPro;

public class TMPWordHoverUI : MonoBehaviour
{
    public Camera uiCamera; // Assign the camera that renders your Canvas (usually the Main Camera if Screen Space - Camera)
    [SerializeField]
    private TMP_Text textMeshPro;
    private int lastWordIndex = -1;

    void Awake()
    {
        
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Detect word under cursor in UI
        int wordIndex = TMP_TextUtilities.FindIntersectingWord(textMeshPro, mousePosition, uiCamera);

        if (wordIndex != -1 && wordIndex != lastWordIndex)
        {
            // Entered a new word
            string word = textMeshPro.textInfo.wordInfo[wordIndex].GetWord();
            Debug.Log("Hovering over word: " + word);

            // TODO: call your interaction function here
        }
        else if (wordIndex == -1 && lastWordIndex != -1)
        {
            // Exited a word
            Debug.Log("No longer hovering over word.");
        }

        lastWordIndex = wordIndex;
    }
}
