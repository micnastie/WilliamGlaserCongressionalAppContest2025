using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerMovment : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    public AudioClip walkingSound; // Assign in inspector

    private Rigidbody rb;
    private AudioSource audioSource;
    private float xRotation = 0f;
    private float ignoreMouseInputTime = 0.1f;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        playerCamera.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (timer < ignoreMouseInputTime)
        {
            timer += Time.deltaTime;
            return; // Skip mouse input initially
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ);
        if (move.magnitude > 1f)
            move = move.normalized;

        Vector3 moveRelative = transform.TransformDirection(move);
        rb.velocity = new Vector3(moveRelative.x * speed, rb.velocity.y, moveRelative.z * speed);

       
    }

    
}
