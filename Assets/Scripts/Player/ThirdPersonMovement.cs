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
    public event Action StartSprinting = delegate { };

    [Header("References")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cam;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] float speed = 6;
    [SerializeField] float sprintSpeed = 12;
    [SerializeField] float turnSmoothing = 0.1f;
    float turnSmoothVelocity;
    // bool isSprinting;

    [Header("Physics Settings")]
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] bool isGrounded;
    Vector3 playerVerticalVelocity;
    public Vector3 controllerVelocity;

    // animation checks
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
        Movement();
    }

    void Movement()
    {
        controllerVelocity = characterController.velocity;
        // check if player is on a ground layer
        isGrounded = Physics.CheckSphere(groundChecker.position, 0.1f, groundLayer);

        // apply horizontal movement on x and z axes- going forwards/backwards and sideways
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            CheckIfStartedMoving();

            // rotate based on camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // movement- turn rotation into direction
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                characterController.Move(moveDir.normalized * speed * Time.deltaTime);
                isSprinting = false;
            }
            // sprint
            else
            {
                characterController.Move(moveDir.normalized * sprintSpeed * Time.deltaTime);
                isSprinting = true;
            }
        }
        else
        {
            CheckIfStoppedMoving();
        }

        // applying vertical movement on y axis- jumping and gravity
        if (isGrounded)
        {
            if (playerVerticalVelocity.y < 0)
                playerVerticalVelocity.y = 0;

            CheckIfStoppedFalling();
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
        }

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
                if (!isSprinting)
                {
                    StartRunning?.Invoke();
                }
                else
                {
                    StartSprinting?.Invoke();
                }
            }

            isMoving = true;
        }
    }

    // invoke idle when player stops moving while grounded or after being airborne
    void CheckIfStoppedMoving()
    {
        if (isGrounded)
        {
            if (isMoving || !isFalling)
                Idle?.Invoke();

            isMoving = false;
        }
    }

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
    }
}
