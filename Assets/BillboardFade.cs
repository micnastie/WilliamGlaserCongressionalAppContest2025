using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Animator))]
public class BillboardFade : MonoBehaviour
{
    public string playerTag = "Player";        // Tag on player

    [Header("Fading Settings")]
    public float fadeStartDistance = 5f;       // Distance where fading begins
    public float minFadeDistance = 1f;         // Distance where alpha is at minimum
    public float minAlpha = 0.2f;              // Alpha when player is within minFadeDistance
    public float baseAlpha = 1f;               // Alpha when player is farther than fadeStartDistance

    [Header("Rotation Settings")]
    public Vector3 rotationOffset = Vector3.zero; // Offset rotation in degrees (X, Y, Z)

    [Header("Animation Settings")]
    public bool playAnimation = false;          // Should we play animation?
    public bool coinFlipCheck = false;          // Random chance at start to play or not
    public string animationStateName = "Idle";  // Animation state to play
    public float minAnimSpeed = 0.8f;           // Minimum random speed
    public float maxAnimSpeed = 1.2f;           // Maximum random speed

    private Transform player;
    private Material mat;
    private MeshCollider meshCollider;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        Renderer renderer = GetComponent<Renderer>();
        meshCollider = GetComponent<MeshCollider>();
        animator = GetComponent<Animator>();

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

        // --- Animation Setup ---
        if (playAnimation && animator != null)
        {
            if (coinFlipCheck)
            {
                bool play = Random.value > 0.5f;
                if (!play)
                {
                    return; // Do not play animation
                }
            }

            // Set random animation speed
            animator.speed = Random.Range(minAnimSpeed, maxAnimSpeed);
            animator.Play(animationStateName, -1, 0f);
        }
    }

    void Update()
    {
        if (player == null || mat == null) return;

        // --- BILLBOARD AROUND Z AXIS ---
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // lock tilt

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
        }

        // --- DISTANCE-BASED TRANSPARENCY ---
        float distance = Vector3.Distance(transform.position, player.position);
        float alpha = baseAlpha;

        if (distance < fadeStartDistance)
        {
            if (distance <= minFadeDistance)
            {
                alpha = minAlpha; // fully faded inside minimum zone
            }
            else
            {
                // Lerp between baseAlpha and minAlpha
                float t = (distance - minFadeDistance) / (fadeStartDistance - minFadeDistance);
                alpha = Mathf.Lerp(minAlpha, baseAlpha, t);
            }
        }

        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        // --- COLLIDER TOGGLE ---
        if (meshCollider != null)
        {
            meshCollider.enabled = (alpha > minAlpha + 0.01f);
        }
    }
}
