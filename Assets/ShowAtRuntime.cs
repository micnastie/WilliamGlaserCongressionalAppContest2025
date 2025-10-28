using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShowAtRuntime : MonoBehaviour
{
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // Hide in scene by default
        rend.enabled = false;
    }

    void Start()
    {
        // Enable renderer when game starts
        rend.enabled = true;
    }
}
