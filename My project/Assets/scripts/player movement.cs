using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    private bool isGrounded;

    [Header("Camera Settings")] 
    public Transform cameraTarget;
    public float cameraSmoothSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 7, -10);
    public float mouseSensitivity = 2f;
    public float minCameraDistance = 15f; // Minimum distance between camera and player
    private Camera mainCamera;
    private float horizontalRotation = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTarget == null)
        {
            // Create camera target if not assigned
            GameObject target = new GameObject("CameraTarget");
            cameraTarget = target.transform;
            cameraTarget.parent = transform;
            cameraTarget.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        HandleCameraMovement(); // Handle camera first
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // Get keyboard input
        float horizontal = 0;
        if (Input.GetKey(KeyCode.A))
            horizontal = -1;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1;
        float vertical = Input.GetAxisRaw("Vertical");

        // Calculate movement direction relative to camera's forward direction
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0; // Keep movement on horizontal plane
        forward = forward.normalized;
        
        Vector3 right = mainCamera.transform.right;
        right.y = 0; // Keep movement on horizontal plane
        right = right.normalized;

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        // Move player without rotating
        if (moveDirection != Vector3.zero)
        {
            // Move player
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    void HandleJump()
    {
        // Simple ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Vector3 jumpVector = Vector3.up * jumpForce;
            transform.position += jumpVector * Time.deltaTime;
        }
    }

    void HandleCameraMovement()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update horizontal rotation based on mouse X movement
        horizontalRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Calculate desired camera position
        Vector3 desiredPosition = cameraTarget.position + cameraTarget.rotation * cameraOffset;

        // Check for obstacles between camera and target
        RaycastHit hit;
        Vector3 directionToCamera = (desiredPosition - cameraTarget.position).normalized;
        float distanceToTarget = Vector3.Distance(cameraTarget.position, desiredPosition);
        
        if (Physics.Raycast(cameraTarget.position, directionToCamera, out hit, distanceToTarget))
        {
            // If there's an obstacle, position camera at hit point with minimum distance
            float adjustedDistance = Mathf.Max(hit.distance - 0.5f, minCameraDistance);
            desiredPosition = cameraTarget.position + directionToCamera * adjustedDistance;
        }

        // Smoothly move camera
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            cameraSmoothSpeed * Time.deltaTime
        );

        // Make camera look at target
        mainCamera.transform.LookAt(cameraTarget.position);
    }
}
