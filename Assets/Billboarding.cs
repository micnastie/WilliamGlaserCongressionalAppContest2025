using UnityEngine;

public class Billboarding : MonoBehaviour
{
    public Transform player;                // assign in inspector or auto-find
    public float triggerDistance = 1f;
    public Vector3 rotationOffset = new Vector3(90, 0, 0);

    public DialologManager dm;
    public QuestManagerScript qm;
    public Conversation loadNext;
    public DialologManager.speakerEnum speaker;
    [Header("UI Prompt")]
    public GameObject pressButtonUI;        // assign in inspector (e.g. "Press E")

    private bool inRange = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        dm = GameObject.Find("DialogManerObj").GetComponent<DialologManager>();
        qm = GameObject.Find("QuestManager").GetComponent<QuestManagerScript>();

        if (pressButtonUI != null)
            pressButtonUI.SetActive(false);
    }

    private void Update()
    {
        // Billboard toward player
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPosition);
        transform.Rotate(rotationOffset);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        inRange = distance <= triggerDistance;

        // Show prompt if in range & no convo
        if (pressButtonUI != null)
            pressButtonUI.SetActive(inRange && !dm.ongoingConvo);

        // Only trigger when in range, key pressed, and no convo running
        if (inRange && !dm.ongoingConvo && Input.GetKeyDown(KeyCode.E))
        {
            if (qm.QuestActive)
            {
                //Debug.Log(qm.currentQuest.notYetDoneConvo.otherSpeaker + " : " + speaker);
                
                if (qm.pickedUp && qm.currentQuest.continuingConvo.otherSpeaker==speaker)
                {
                    StartCoroutine(dm.loadConvo(qm.currentQuest.continuingConvo));
                }
                else if (qm.currentQuest.notYetDoneConvo.otherSpeaker == speaker)
                {
                    //loadNPCbasicDialog;
                    Debug.Log("yapssss1");
                    StartCoroutine(dm.loadConvo(qm.currentQuest.notYetDoneConvo,true));
                    
                }
                else
                {
                    Debug.Log("yap1");
                }
            }
            else
            {
                if (loadNext.otherSpeaker == speaker) { StartCoroutine(dm.loadConvo(loadNext)); }
                else
                {
                    //loadNPCbasicDialog;
                    Debug.Log("yap2");
                }
                
            }
        }
    }
}
