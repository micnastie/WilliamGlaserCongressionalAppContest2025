using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool QuestActive;
    public bool pickedUp;
    public Quest currentQuest;
    public GameObject tm;//tracking manager
    public AudioManager au;
    
    public void loadQuest(Quest questToLoad)
    {
        QuestActive = true;
        pickedUp= false;
        currentQuest = questToLoad;
        GameObject x = Instantiate(currentQuest.prefabToSpawn, new Vector3(0,0,0), Quaternion.identity);
        tm.GetComponent<ObjectiveArrow>().objective = x.transform;

    }
    public void endQuest()
    {
        pickedUp= true;
        au.PlaySFX("jingles_SAX16");
        tm.GetComponent<ObjectiveArrow>().objective = GameObject.Find("NPC" + (int)currentQuest.continuingConvo.otherSpeaker).transform;
    }

}
