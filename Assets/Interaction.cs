using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Interaction : ScriptableObject
{
    // Start is called before the first frame update

    public string[] tooltipWords;
    public string[] tooltips;

    public DialologManager.speakerEnum ThisSpeaker;
    public string dialog;
    public bool hasQuestion;
    
    public string correct, wrong,wrongDialog,correctDialog, followUpDialog, wrongAnswerHelp;


 
}
