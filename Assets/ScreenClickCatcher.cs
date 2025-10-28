using UnityEngine;

public class ScreenClickCatcher : MonoBehaviour
{
    // Assign this from your coroutine or manager
    public System.Action onClick;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse button
        {
            onClick?.Invoke();
        }
    }
}
