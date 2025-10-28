using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialologManager : MonoBehaviour
{
    private TalkingManager tk;
    private Coroutine currentBlipCoroutine;
    [SerializeField]
    private bool isTyping = false;
    private bool blipPlaying = false;


    // start is called before the first frame update
    [System.Serializable]
    public class SpeakerVoice
    {
        public DialologManager.speakerEnum speaker;
        public float basePitch = 1.0f;
    }

    public List<SpeakerVoice> speakerVoices;
    private Dictionary<speakerEnum, float> pitchDict;


    [Header("Voice Settings")]
    public List<AudioClip> blipClips; // one list for all characters
    private AudioSource audioSource;
    private AudioClip lastClip;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        pitchDict = new Dictionary<speakerEnum, float>();
        foreach (var sv in speakerVoices)
        {
            pitchDict[sv.speaker] = sv.basePitch;
        }
    }




    public enum speakerEnum
    {
        Me,
        Anna,
        Frau_Müller,
        Herr_Becker
    }
    public bool isReviewing = false;
    public List<Conversation> wrongConvos;
    public Conversation currentConvo;
    public bool ongoingConvo;
    public GameObject canvas;
    [SerializeField]
    private GameObject next;
    [SerializeField]
    private GameObject leftImage, rightImage, leftButton, rightButton,tooltipObj;
    [SerializeField]
    private GameObject leftNameText, rightNameText, dialogText, dialogbox, background, winPanel;
    public int currentInteraction;
    [SerializeField]
    private Sprite[] spriteList;
    public bool clicked;
    public int lastInput;
    public ScreenClickCatcher clickCatcher;
    public string[] wordsWithTT, ttList;// DialologManager.wordsWithTT
    [SerializeField]
    private GameObject questmanager;
    public AudioManager au;
    public GameObject player;
    public GameObject reivewButton;
    public ScreenFade screenfader;
    [SerializeField]
    private GameObject cueinreview;
    public float textSpeed = 0.05f;
    
    void Start()
    {
        clickCatcher.onClick += nextDialog;
        tk = GameObject.FindAnyObjectByType<TalkingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator loadConvo(Conversation conversationToLoad, bool Yap = false)
    {
        StopCoroutine("PlayVoiceBlipAsync");
        au.PlaySFX("open_001");
        tk.SetTalkingVolume(true);
        if(isReviewing)cueinreview.SetActive(true);
        if (conversationToLoad.endQuest)
        {
            //endquest
            questmanager.GetComponent<QuestManagerScript>().pickedUp = false;
            questmanager.GetComponent<QuestManagerScript>().QuestActive = false;
        }
        ongoingConvo= true;
        Debug.Log(ongoingConvo);
        currentInteraction = 0;
        currentConvo= conversationToLoad;
        player.GetComponent<PlayerMovment>().enabled= false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dialogbox.SetActive(true);
        leftImage.GetComponent<Image>().sprite = spriteList[0];
        Debug.Log((int)conversationToLoad.otherSpeaker);
        rightImage.GetComponent<Image>().sprite = spriteList[(int)conversationToLoad.otherSpeaker];
        if(conversationToLoad.otherSpeaker != speakerEnum.Me){
            if(conversationToLoad.otherSpeaker == speakerEnum.Frau_Müller)
            {
                rightNameText.GetComponent<TextMeshProUGUI>().text = "Fr. Müller";
            }
            else if(conversationToLoad.otherSpeaker == speakerEnum.Herr_Becker)
            {
                rightNameText.GetComponent<TextMeshProUGUI>().text = "Hr. Becker";
            }
            else
            {
                rightNameText.GetComponent<TextMeshProUGUI>().text = "Anna";
            }

           
        }

       
        clickCatcher.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        tooltipObj.SetActive(false);
        for(int i = 0; i <conversationToLoad.interactionArr.Length; i++)
        {
            Debug.Log("CONVO INTERACTION: "+i);
            yield return StartCoroutine(loadInteraction(conversationToLoad.interactionArr[i]));
           
        }
        //loadQuest;
        if (!isReviewing)
        {
            if (conversationToLoad.nextConvoWhenBroken !=null)
            {
                //breakout
                GameObject[] billboarders = GameObject.FindGameObjectsWithTag("NPC");
                for(int i = 0; i < billboarders.Length; i++)
                {
                    billboarders[i].GetComponent<Billboarding>().loadNext = conversationToLoad.nextConvoWhenBroken;
                }

            }
            else if (conversationToLoad.questToLoad != null)
            {//HAS QUEST
                questmanager.GetComponent<QuestManagerScript>().loadQuest(conversationToLoad.questToLoad);

            }
        }


        if (conversationToLoad.nextConvoWhenBroken != null)
        {
            GameObject.FindGameObjectWithTag("arrow").GetComponent<ObjectiveArrow>().objective = GameObject.Find("NPC" + (int)currentConvo.nextConvoWhenBroken.otherSpeaker).transform;
        }
        if (isReviewing)
        {
            // close dialog cleanly without win screen
            dialogbox.SetActive(false);
            ongoingConvo = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (conversationToLoad.questToLoad == null && conversationToLoad.nextConvoWhenBroken == null && !Yap)
        {
            senarioFinished();
        }
        else
        {
            Debug.Log("CONVO FIN");
            au.PlaySFX("minimize_008");
            dialogbox.SetActive(false);
            ongoingConvo = false;
            player.GetComponent<PlayerMovment>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        tk.SetTalkingVolume(false);
        Debug.Log("DONE");
    }
    public IEnumerator loadInteraction(Interaction interactionToLoad)
    {
        /*
         * set image to person talking
         * set dialog
         * wait
         * if choice, present
         
         */
        if (!isReviewing)
        {
            Debug.Log("Resetting background to white, isReviewing = " + isReviewing);
            background.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);

        }
        else
        {
            Debug.Log("Skipped resetting background, isReviewing = " + isReviewing);
        }

        Debug.Log(interactionToLoad.ThisSpeaker);
        if(interactionToLoad.ThisSpeaker == speakerEnum.Me)
        {

            setImageAlpha(0, 1f);//set me to talk
            setImageAlpha(1, .5f);
        }
        else
        {
            setImageAlpha(1, 1f);//otherway
            setImageAlpha(0, .5f);
        }
        //dialogText.GetComponent<TextMeshProUGUI>().text = interactionToLoad.dialog;
        yield return StartCoroutine(TypeText(interactionToLoad.dialog, dialogText.GetComponent<TextMeshProUGUI>(), interactionToLoad));
        //wait
        clicked = false;
        next.SetActive(true);
        clickCatcher.gameObject.SetActive(true);
        
        pullWords(interactionToLoad);
        Debug.Log("[WaitUntil] Waiting for clicked to become true");
        yield return new WaitUntil(() => clicked);
        Debug.Log("[WaitUntil] clicked is true, continuing");

        au.PlaySFX("click_001");
        next.SetActive(false);
        if (interactionToLoad.hasQuestion)
        {
            clicked = false;
            lastInput = -1;
            //RANDOMIZE POSITIONS OF BUTTONS
            leftButton.SetActive(true);
            leftButton.GetComponentInChildren<TextMeshProUGUI>().text = interactionToLoad.correct;
           
            rightButton.SetActive(true);
            rightButton.GetComponentInChildren<TextMeshProUGUI>().text = interactionToLoad.wrong;
            if (Random.value < 0.5f)
            {
                Vector3 tempPosition = leftButton.transform.position;
                leftButton.transform.position = rightButton.transform.position;
                rightButton.transform.position = tempPosition;
            }
            pullWords(interactionToLoad);
            Debug.Log("[WaitUntil] Waiting for clicked to become true");
            yield return new WaitUntil(() => clicked);
            Debug.Log("[WaitUntil] clicked is true, continuing");

            rightButton.SetActive(false);
            leftButton.SetActive(false);
           
            setImageAlpha(1, 1f);//otherway
            setImageAlpha(0, .5f);
            if (lastInput == 0) {//ALWAYS CHANGE TO OTHER SPEAKER
                //correct
                //dialogText.GetComponent<TextMeshProUGUI>().text = interactionToLoad.correctDialog;
                pullWords(interactionToLoad);
                yield return StartCoroutine(TypeText(interactionToLoad.correctDialog, dialogText.GetComponent<TextMeshProUGUI>(), interactionToLoad,true));
            }
            else if (lastInput == 1) // wrong answer
            {
                if (!isReviewing)
                {
                    if (!wrongConvos.Contains(currentConvo))
                        wrongConvos.Add(currentConvo);
                }
                else
                {
                   
                    wrongConvos.Remove(currentConvo);
                    wrongConvos.Add(currentConvo);
                }

                pullWords(interactionToLoad);
                yield return StartCoroutine(TypeText(interactionToLoad.wrongDialog, dialogText.GetComponent<TextMeshProUGUI>(), interactionToLoad, true));
            }
            else
            {
                //if its here  fuckd up ur code;
            }
           
            clicked = false;
            next.SetActive(true);
            clickCatcher.gameObject.SetActive(true);
            Debug.Log("[WaitUntil] Waiting for clicked to become true");
            yield return new WaitUntil(() => clicked);
            Debug.Log("[WaitUntil] clicked is true, continuing");


            au.PlaySFX("click_001");
            next.SetActive(false);
            //next
            setImageAlpha(0, 1f);//set me to talk
            setImageAlpha(1, .5f);



        }
        else
        {
            //none
        }


        Debug.Log("FINISHED A INTERACTION");

    }

    public void setImageAlpha(int welchesBild, float alpha)
    {
        Image img = (welchesBild == 0)
            ? leftImage.GetComponent<Image>()
            : rightImage.GetComponent<Image>();

        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    public void buttonPress(int buttonNum)
    {
        lastInput = buttonNum;
        clicked = true;

        if (buttonNum == 0) { au.PlaySFX("confirmation_001"); if (isReviewing) return;
            background.GetComponent<Image>().color = new Color(181f/225f, 255f/225f, 168f / 225f, .5f);
        }
        else
        {
            au.PlaySFX("error_002");
            if (isReviewing) return;
            background.GetComponent<Image>().color = new Color(246f / 225f, 127f / 225f, 103f / 225f, .5f);
            
        }

    }
    public void nextDialog()
{
    Debug.Log("CLICKED");

    clicked = true;
    clickCatcher.gameObject.SetActive(false);

    
    isTyping = false;      // stop spawning new blips
    blipPlaying = false;   // allow any current blip coroutine to finish and not spawn another
    audioSource.Stop();    // cut any currently playing blip
}
    public void pullWords(Interaction interactionToLoad)
    {
        wordsWithTT = interactionToLoad.tooltipWords;
        ttList = interactionToLoad.tooltips;
        dialogText.GetComponent<TMP_UI_WordHoverHighlight>().RegatherWords();
    }

    public IEnumerator endOfDayReview()
    {
        isReviewing = true;
        winPanel.gameObject.SetActive(false);
       // FF55E1
        background.GetComponent<Image>().color = ColorUtility.TryParseHtmlString("#E200FF80", out Color c) ? c : Color.white;
        GameObject.Find("grid").GetComponent<Image>().color = ColorUtility.TryParseHtmlString("#FF55E1FF", out c) ? c : Color.white;

       
        for (int i = 0; i < wrongConvos.Count; i++)
        {
            Conversation convo = wrongConvos[i];
            Debug.Log(convo + " STARTING CONVO REVIEW");
            yield return StartCoroutine(loadConvo(convo));
            Debug.Log("A REVIEW FINISHED!");
        }

        isReviewing = false;
    }


    private IEnumerator TypeText(string textToShow, TextMeshProUGUI textComponent, Interaction interactionToLoad, bool isResponse = false)
    {

        textComponent.text = "";
        clicked = false;
        isTyping = true;
        clickCatcher.gameObject.SetActive(true);
        DialologManager.speakerEnum speaker = interactionToLoad.ThisSpeaker;
        if(interactionToLoad.ThisSpeaker == speakerEnum.Me&&!isResponse)
        {
            leftImage.transform.localRotation = Quaternion.Euler(0, 0, -7);

        }
        else
        {
            rightImage.transform.localRotation = Quaternion.Euler(0, 0, 7);
        }
        for (int i = 0; i < textToShow.Length; i++)
        {
            if (!isTyping) //  skip trigered
            {
                textComponent.text = textToShow; // imediately show all text
                break; // exit th loop
            }
            textComponent.text += textToShow[i];
            pullWords(interactionToLoad);

            // only start a new blip if typing is ongoing
            if (i % 2 == 0 && i < textToShow.Length - 1 && isTyping)
            {
                if (!blipPlaying) // start a new blip only if previous finished
                    StartCoroutine(PlayVoiceBlipAsync(speaker));
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false; // typing finished
        clickCatcher.gameObject.SetActive(false);
        clicked = false;
        leftImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
        rightImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
        isTyping = false;
        audioSource.Stop(); // stop any remaining blips

    }


    private IEnumerator PlayVoiceBlipAsync(speakerEnum speaker)
    {
        if (!isTyping) yield break; // exit if skip already triggered
        if (blipClips.Count == 0) yield break;

        blipPlaying = true;

        AudioClip clip = GetRandomClip();
        if (clip == null) { blipPlaying = false; yield break; }

        float basePitch = pitchDict.ContainsKey(speaker) ? pitchDict[speaker] : 1.0f;
        float pitch = basePitch + Random.Range(-0.05f, 0.05f);

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);

        
        float timer = 0f;
        while (timer < clip.length / pitch)
        {
            if (!isTyping) { blipPlaying = false; yield break; }
            timer += Time.deltaTime;
            yield return null;
        }

        blipPlaying = false;

        // only continue if typing is still ongoing
        if (isTyping)
            StartCoroutine(PlayVoiceBlipAsync(speaker));
        
    }




    private AudioClip GetRandomClip()
    {
        if (blipClips.Count == 0) return null;

        AudioClip clip;
        do
        {
            clip = blipClips[Random.Range(0, blipClips.Count)];
        } while (clip == lastClip && blipClips.Count > 1);

        lastClip = clip;
        return clip;
    }


    private void senarioFinished()
    {
        winPanel.SetActive(true);
        if(wrongConvos.Count>0)reivewButton.SetActive(true);
    }
    public void goToMainmenu()
    {
        StartCoroutine(fadeToMainMenu());
    }

    public IEnumerator fadeToMainMenu()
    {

        screenfader.FadeOut();
        yield return new WaitForSeconds(screenfader.fadeDuration + 0.1f);
        SceneManager.LoadScene("MainMenu");
    }

}
