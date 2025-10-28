using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class Conversation : ScriptableObject
{
    // Start is called before the first frame update
    public Interaction[] interactionArr;
    public DialologManager.speakerEnum otherSpeaker;
    public Quest questToLoad;
    public bool endQuest;
    public Conversation nextConvoWhenBroken;
}
