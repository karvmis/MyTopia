using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f; // Sprint nopeus
    public float rotationSpeed = 10f;
    public float jumpForce = 10f;
    public float gravity = 10f;
    
    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f;
    public float groundCheckYOffset = 0.1f;  // Säätää raycastin aloituspistettä
    public LayerMask groundLayer;          // Kerros, jonka kanssa tarkistetaan maa
    private bool isGrounded;
    private float verticalVelocity = 1f;

    [Header("Fall Damage Settings")]
    public float fallDamageThreshold = 5f; // Minimipudotusmatka ennen kuin vahinko alkaa
    public float fallDamageMultiplier = 10f; // Vahingon kertymiskerroin ylimenevälle pudotusmatkalle
    private float fallStartHeight;
    private bool isFalling = false;

    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float cameraSmoothSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 7, -10);
    public float mouseSensitivity = 2f;
    public float minCameraDistance = 15f; // Pienin etäisyys kameran ja pelaajan välillä
    private Camera mainCamera;
    private float horizontalRotation = 0f;

    // Viite pelaajan terveyskomponenttiin
    private PlayerHealth playerHealth;

    void Start()
    {
        mainCamera = Camera.main;

        // Lukitaan kursori ja piilotetaan se
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTarget == null)
        {
            GameObject target = new GameObject("CameraTarget");
            cameraTarget = target.transform;
            cameraTarget.parent = transform;
            cameraTarget.localPosition = Vector3.zero;
        }

        // Haetaan pelaajan terveyskomponentti
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth-komponenttia ei löytynyt!");
        }
    }

    void Update()
    {
        HandleCameraMovement();
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float horizontal = 0;
        if (Input.GetKey(KeyCode.A))
            horizontal = -1;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1;
        float vertical = Input.GetAxisRaw("Vertical");

        // Sprint-toiminto: käytetään speedia jos Shift nappia painetaan
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = sprintSpeed;
        }

        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right = right.normalized;

        Vector3 moveDirection = forward * vertical + right * horizontal;
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }

        if (moveDirection != Vector3.zero)
        {
            Vector3 currentPos = transform.position;
            Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;
            transform.position = new Vector3(
                currentPos.x + movement.x,
                currentPos.y,
                currentPos.z + movement.z
            );
        }
    }

    void HandleJump()
    {
        // Käynnistetään raycast hieman alaspäin, jotta ei osuta omaan kollideriin
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * groundCheckYOffset;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance + groundCheckYOffset, groundLayer);

        Debug.Log($"IsGrounded: {isGrounded}, VerticalVelocity: {verticalVelocity}, Height: {transform.position.y}");
        Debug.DrawRay(rayOrigin, Vector3.down * (groundCheckDistance + groundCheckYOffset), Color.red);

        // Käytetään painovoimaa
        verticalVelocity -= gravity * Time.deltaTime;

        // Tallennetaan pudotuksen aloituskorkeus, jos pelaaja alkaa pudota
        if (!isGrounded && verticalVelocity < 0 && !isFalling)
        {
            fallStartHeight = transform.position.y;
            isFalling = true;
        }

        if (isGrounded)
        {
            // Lasketaan pudotusmatka ja mahdollinen vahinko
            if (isFalling)
            {
                float fallDistance = fallStartHeight - hit.point.y;
                if (fallDistance > fallDamageThreshold)
                {
                    int damage = Mathf.RoundToInt((fallDistance - fallDamageThreshold) * fallDamageMultiplier);
                    Debug.Log($"Pudotusvahinko: {damage} (Pudotusmatka: {fallDistance})");
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                    }
                }
                isFalling = false;
            }

            if (verticalVelocity < 0)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                verticalVelocity = 0f;
            }

            // Hyppy vain kun ollaan maassa
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Hyppy!");
                verticalVelocity = jumpForce;
                isGrounded = false;
            }
        }

        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
    }

    void HandleCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        horizontalRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        Vector3 desiredPosition = cameraTarget.position + cameraTarget.rotation * cameraOffset;

        RaycastHit hit;
        Vector3 directionToCamera = (desiredPosition - cameraTarget.position).normalized;
        float distanceToTarget = Vector3.Distance(cameraTarget.position, desiredPosition);
        
        if (Physics.Raycast(cameraTarget.position, directionToCamera, out hit, distanceToTarget))
        {
            float adjustedDistance = Mathf.Max(hit.distance - 0.5f, minCameraDistance);
            desiredPosition = cameraTarget.position + directionToCamera * adjustedDistance;
        }

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            cameraSmoothSpeed * Time.deltaTime
        );

        mainCamera.transform.LookAt(cameraTarget.position);
    }
}
