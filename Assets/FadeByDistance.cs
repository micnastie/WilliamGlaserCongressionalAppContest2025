using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(MeshCollider))]
public class FadeByDistance : MonoBehaviour
{
    public string playerTag = "Player";        // Tag of the player
    public float fadeStartDistance = 5f;       // Distance where fading begins
    public float minFadeDistance = 1f;         // Distance where alpha is at minimum
    public float minAlpha = 0.2f;              // Alpha when player is within minFadeDistance
    public float baseAlpha = 1f;               // Alpha when player is farther than fadeStartDistance

    private Transform player;
    private Material mat;
    private MeshCollider meshCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        Renderer renderer = GetComponent<Renderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (renderer != null)
        {
            mat = renderer.material;

            // Force transparency if using Standard Shader
            if (mat.shader.name == "Standard")
            {
                mat.SetFloat("_Mode", 3); // Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }

        if (player == null)
        {
            Debug.LogError("No player found with tag: " + playerTag);
        }
    }

    void Update()
    {
        if (player == null || mat == null) return;

        // Distance-based transparency
        float distance = Vector3.Distance(transform.position, player.position);
        float alpha = baseAlpha;

        if (distance < fadeStartDistance)
        {
            if (distance <= minFadeDistance)
            {
                alpha = minAlpha;
            }
            else
            {
                float t = (distance - minFadeDistance) / (fadeStartDistance - minFadeDistance);
                alpha = Mathf.Lerp(minAlpha, baseAlpha, t);
            }
        }

        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        // Toggle collider based on visibility
        if (meshCollider != null)
        {
            meshCollider.enabled = (alpha > minAlpha + 0.01f);
        }
    }
}
