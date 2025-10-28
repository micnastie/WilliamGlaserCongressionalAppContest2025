using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(TMP_Text))]
public class TMP_UI_WordHoverHighlight : MonoBehaviour
{
    [Header("References")]
    public DialologManager dialogManager;    
    public GameObject tooltipBox;            

    [Header("Colors")]
    public Color32 tooltipWordColor = new Color32(0, 200, 255, 255);
    public Color32 hoverColor = new Color32(255, 240, 0, 255);       
    //REALL
    [Header("Optional")]
    public Camera overrideCamera;

    private TMP_Text tmp;
    private Canvas rootCanvas;
    private Camera uiCam;
    private int lastWordIndex = -1;
    private TMP_MeshInfo[] cachedMeshInfo;

    private TMP_Text tooltipWordText;
    private TMP_Text tooltipTTText;

    private Dictionary<string, string> wordToTooltip;

    [SerializeField]
    private AudioManager au;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        rootCanvas = GetComponentInParent<Canvas>();

        if (tooltipBox != null)
        {
            tooltipWordText = tooltipBox.transform.Find("Word").GetComponent<TMP_Text>();
            tooltipTTText = tooltipBox.transform.Find("tt").GetComponent<TMP_Text>();
        }

        RegatherWords();
    }

    void Start()
    {
        ResolveCamera();
        RefreshText();
        tooltipBox.SetActive(false);
    }

    void ResolveCamera()
    {
        if (overrideCamera != null) { uiCam = overrideCamera; return; }
        if (rootCanvas == null) { uiCam = null; return; }

        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            uiCam = null;
        else
            uiCam = rootCanvas.worldCamera != null ? rootCanvas.worldCamera : Camera.main;
    }

    void RefreshText()
    {
        tmp.ForceMeshUpdate();
        cachedMeshInfo = tmp.textInfo.CopyMeshInfoVertexData();

        if (wordToTooltip == null) return;

        // Pre-color any TT words
        for (int w = 0; w < tmp.textInfo.wordCount; w++)
        {
            string word = tmp.textInfo.wordInfo[w].GetWord();
            if (wordToTooltip.ContainsKey(word))
                ColorWord(w, tooltipWordColor);
        }
    }

    void Update()
    {
        if (tmp.havePropertiesChanged)
            RefreshText();

        int wordIndex = TMP_TextUtilities.FindIntersectingWord(tmp, Input.mousePosition, uiCam);

        if (wordIndex != -1 && wordIndex != lastWordIndex)
        {
            string word = tmp.textInfo.wordInfo[wordIndex].GetWord();

            if (wordToTooltip != null && wordToTooltip.TryGetValue(word, out string tooltip))
            {
                RestoreWord(lastWordIndex);
                HighlightWord(wordIndex);

                tooltipWordText.text = word;
                tooltipTTText.text = tooltip;
                tooltipBox.SetActive(true);
                au.PlaySFX("click_002");
            }
            else
            {
                RestoreWord(lastWordIndex);
                if(tooltipBox.activeSelf==true)au.PlaySFX("click_003");
                tooltipBox.SetActive(false);
                
            }
        }
        else if (wordIndex == -1 && lastWordIndex != -1)
        {
            RestoreWord(lastWordIndex);
            if (tooltipBox.activeSelf == true) au.PlaySFX("click_003");
            tooltipBox.SetActive(false);
        }

        lastWordIndex = wordIndex;
    }

    
    public void RegatherWords()
    {
        Debug.Log("REGATHERED WORDS");
        if (dialogManager != null && dialogManager.wordsWithTT != null && dialogManager.ttList != null)
        {
            wordToTooltip = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < dialogManager.wordsWithTT.Length; i++)
            {
                if (!wordToTooltip.ContainsKey(dialogManager.wordsWithTT[i]))
                    wordToTooltip.Add(dialogManager.wordsWithTT[i], dialogManager.ttList[i]);
            }

         
            RefreshText();
        }
    }

    void HighlightWord(int wordIndex)
    {
        if (wordIndex < 0) return;
        TMP_WordInfo w = tmp.textInfo.wordInfo[wordIndex];

        for (int i = 0; i < w.characterCount; i++)
        {
            int charIndex = w.firstCharacterIndex + i;
            var cInfo = tmp.textInfo.characterInfo[charIndex];
            if (!cInfo.isVisible) continue;

            int matIndex = cInfo.materialReferenceIndex;
            int vertIndex = cInfo.vertexIndex;

            var colors = tmp.textInfo.meshInfo[matIndex].colors32;
            colors[vertIndex + 0] = hoverColor;
            colors[vertIndex + 1] = hoverColor;
            colors[vertIndex + 2] = hoverColor;
            colors[vertIndex + 3] = hoverColor;
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void ColorWord(int wordIndex, Color32 color)
    {
        if (wordIndex < 0) return;
        TMP_WordInfo w = tmp.textInfo.wordInfo[wordIndex];

        for (int i = 0; i < w.characterCount; i++)
        {
            int charIndex = w.firstCharacterIndex + i;
            var cInfo = tmp.textInfo.characterInfo[charIndex];
            if (!cInfo.isVisible) continue;

            int matIndex = cInfo.materialReferenceIndex;
            int vertIndex = cInfo.vertexIndex;

            var colors = tmp.textInfo.meshInfo[matIndex].colors32;
            colors[vertIndex + 0] = color;
            colors[vertIndex + 1] = color;
            colors[vertIndex + 2] = color;
            colors[vertIndex + 3] = color;
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void RestoreWord(int wordIndex)
    {
        if (wordIndex < 0 || cachedMeshInfo == null) return;
        TMP_WordInfo w = tmp.textInfo.wordInfo[wordIndex];
        string word = w.GetWord();

        bool isTooltipWord = wordToTooltip != null && wordToTooltip.ContainsKey(word);

        for (int i = 0; i < w.characterCount; i++)
        {
            int charIndex = w.firstCharacterIndex + i;
            var cInfo = tmp.textInfo.characterInfo[charIndex];
            if (!cInfo.isVisible) continue;

            int matIndex = cInfo.materialReferenceIndex;
            int vertIndex = cInfo.vertexIndex;
            var dst = tmp.textInfo.meshInfo[matIndex].colors32;

            if (isTooltipWord)
            {
                dst[vertIndex + 0] = tooltipWordColor;
                dst[vertIndex + 1] = tooltipWordColor;
                dst[vertIndex + 2] = tooltipWordColor;
                dst[vertIndex + 3] = tooltipWordColor;
            }
            else
            {
                var src = cachedMeshInfo[matIndex].colors32;
                dst[vertIndex + 0] = src[vertIndex + 0];
                dst[vertIndex + 1] = src[vertIndex + 1];
                dst[vertIndex + 2] = src[vertIndex + 2];
                dst[vertIndex + 3] = src[vertIndex + 3];
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

   

}
