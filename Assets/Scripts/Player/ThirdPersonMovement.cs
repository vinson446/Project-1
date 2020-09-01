using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    public event Action Idle = delegate { };
    public event Action StartRunning = delegate { };
    public event Action Jumping = delegate { };
    public event Action Falling = delegate { };
    public event Action Landed = delegate { };
    public event Action StartSprinting = delegate { };

    [Header("References")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cam;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] float runSpeed = 6;
    [SerializeField] float sprintSpeed = 12;
    [SerializeField] float turnSmoothing = 0.1f;
    float turnSmoothVelocity;
    // bool isSprinting;

    [Header("Physics Settings")]
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] bool isGrounded;
    Vector3 playerVerticalVelocity;

    // animation checks (making it public for ez debugging)
    public bool isMoving = false;
    public bool isJumping = false;
    public bool isFalling = false;
    public bool isSprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        Idle?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        VerticalMovement();
        HorizontalMovement();
    }

    // apply horizontal movement on x and z axes- going forwards/backwards and sideways
    void HorizontalMovement()
    {
        // check if player is on a ground layer
        isGrounded = Physics.CheckSphere(groundChecker.position, 0.1f, groundLayer);

        // get user input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        float moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            moveSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            moveSpeed = runSpeed;
        }

        if (direction.magnitude >= 0.1f)
        {
            // rotate based on camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // movement- turn rotation into direction
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            if (isMoving)
            {
                // sprinting
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    StartSprinting?.Invoke();
                }
                // running
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    StartRunning?.Invoke();
                }
            }
            // start movement animation
            else 
            {
                CheckIfStartedMoving();
            }

            characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            CheckIfStoppedMoving();
        }
    }

    // applying vertical movement on y axis- jumping and gravity
    void VerticalMovement()
    {
        if (isGrounded)
        {
            if (playerVerticalVelocity.y < 0)
                playerVerticalVelocity.y = 0;

            if (isFalling)
                CheckIfStoppedFalling();
        }

        // jump
        if (isGrounded && Input.GetButton("Jump"))
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
        }

        // gravity
        playerVerticalVelocity.y += gravity * Time.deltaTime;

        characterController.Move(playerVerticalVelocity * Time.deltaTime);

        if (characterController.velocity.y > 2)
        {
            CheckIfStartedJumping();
        }
        else
        {
            CheckIfStoppedJumping();
            CheckIfStartedFalling();
        }
    }

    // event calls to animators
    // invoke running/sprinting when player is moving while grounded
    void CheckIfStartedMoving()
    {
        if (isGrounded)
        {
            if (!isMoving)
            {
                if (isSprinting)
                {
                    StartSprinting?.Invoke();
                }
                else
                {
                    StartRunning?.Invoke();
                }
            }

            isMoving = true;
        }
    }

    // invoke idle when player stops moving while grounded 
    void CheckIfStoppedMoving()
    {
        if (isGrounded)
        {
            if (isMoving)
                Idle?.Invoke();

            isMoving = false;
        }
    }

    // invoke jumping when player applies upward jump movement
    void CheckIfStartedJumping()
    {
        if (!isGrounded)
        {
            if (!isJumping)
                Jumping?.Invoke();

            isMoving = false;
            isJumping = true;
        }
    }

    void CheckIfStoppedJumping()
    {
        isJumping = false;
    }

    // invoke falling when player's y position is decreasing
    void CheckIfStartedFalling()
    {
        if (!isGrounded)
        {
            if (!isFalling)
                Falling?.Invoke();

            isMoving = false;
            isFalling = true;
        }
    }

    void CheckIfStoppedFalling()
    {
        isFalling = false;

        if (!isMoving)
            Landed?.Invoke();
    }
}
