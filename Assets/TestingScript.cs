using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


public class TestingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dialogmanger;
    public Conversation testConvo;

 public void testLoadConvo()
    {
        StartCoroutine(dialogmanger.GetComponent<DialologManager>().endOfDayReview());

       
    }
    public void Update()
    {
        if(UnityEngine.Input.GetKeyDown(KeyCode.L))testLoadConvo();
    }
}
