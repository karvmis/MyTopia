using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public float cameraDistance = 5f; // Distance of camera from player
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    public float cameraHeight = 4.5f; // Height offset for camera
    public float cameraSmoothness = 1f; // For smooth camera movement

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Initialize camera position
        UpdateCameraPosition();
    }

    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            
            // Clamp the vertical rotation
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
            
            // Rotate the player based on horizontal mouse movement
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
            
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        // Calculate desired camera position
        Vector3 targetPosition = transform.position;
        targetPosition -= transform.forward * cameraDistance; // Move camera behind player
        targetPosition += Vector3.up * cameraHeight; // Add height offset

        // Smoothly move camera to desired position
        playerCamera.transform.position = Vector3.SmoothDamp(
            playerCamera.transform.position,
            targetPosition,
            ref cameraVelocity,
            cameraSmoothness
        );

        // Make camera look at player's head level
        Vector3 lookAtPosition = transform.position + Vector3.up * (characterController.height * 0.8f);
        playerCamera.transform.LookAt(lookAtPosition);
    }
}