using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public GameObject prefabToSpawn;
    public Conversation continuingConvo;
    public Conversation notYetDoneConvo;
}
