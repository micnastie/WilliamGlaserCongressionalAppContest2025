using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManger : MonoBehaviour
{
    [SerializeField]
    private Vector3[] menupos;
    [SerializeField]
    private CameraLerpMover cm;
    public AudioClip buttonpressclip;
    public ScreenFade screenFade; // Assign your ScreenFade in Inspector

    private void Awake()
    {
        cm.gameObject.transform.position = new Vector3(0, 20, -10);
    }

    void Start()
    {
        cm.MoveTo(new Vector3(0, 0, -10));
    }

    public void buttonManger(int WhichMethod)
    {
        AudioSource audio = cm.gameObject.GetComponent<AudioSource>();
        audio.clip = buttonpressclip;
        audio.Play();

        switch (WhichMethod)
        {
            case -3:
                leo();
                break;
            case -2:
                Openmakeit();
                break;
            case -1:
                OpenHolidayPage();
                break;
            case 0:
                cm.MoveTo(menupos[0]);
                break;
            case 1:
                cm.MoveTo(menupos[1]);
                break;
            case 2:
                cm.MoveTo(menupos[2]);
                break;
            case 3:
                cm.MoveTo(menupos[3]);
                break;
            case 4:
                FadeAndLoadScene("Scenario1");
                break;
            case 5:
                cm.MoveTo(menupos[4]);
                break;
        }
    }

    public void OpenHolidayPage()
    {
        Application.OpenURL("https://www.timeanddate.com/holidays/germany/");
    }
    public void Openmakeit()
    {
        Application.OpenURL("https://www.make-it-in-germany.com/en/");
    }
    public void leo()
    {
        Application.OpenURL("https://www.leo.org/german-english/");
    }

    /// <summary>
    /// Call this to fade out and switch scenes
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void FadeAndLoadScene(string sceneName)
    {
        if (screenFade != null)
        {
            StartCoroutine(FadeAndLoadRoutine(sceneName));
        }
        else
        {
            Debug.LogWarning("ScreenFade not assigned! Loading scene immediately.");
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        // Trigger fade-out
        screenFade.FadeOut();

        // Wait for the fade duration
        yield return new WaitForSeconds(screenFade.fadeDuration + 0.1f); // small buffer

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }
}
