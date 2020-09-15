using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    // movement animation events
    public event Action Idle = delegate { };
    public event Action StartRunning = delegate { };
    public event Action Jumping = delegate { };
    public event Action Falling = delegate { };
    public event Action Landed = delegate { };
    public event Action StartSprinting = delegate { };

    // attack animation events
    public event Action ForceImpulse = delegate { };

    [Header("References")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cam;
    Collider characterControllerColl;
    [SerializeField] float colliderYPadding;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] float runSpeed = 6;
    [SerializeField] float sprintSpeed = 12;
    [SerializeField] float turnSmoothing = 0.1f;
    float turnSmoothVelocity;

    bool canMove = true;
    public bool CanMove { get => canMove; set => canMove = value; }

    [Header("Physics Settings")]
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] bool isGrounded;
    Vector3 playerVerticalVelocity;

    // animation checks 
    public bool isMoving { get; private set; }
    public bool isJumping { get; private set; }
    bool isFalling;
    bool isLanding;
    public bool IsLanding { get => isLanding; set => isLanding = value; }
    public bool isSprinting { get; private set; }

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    private void Awake()
    {
        characterControllerColl = characterController.GetComponent<Collider>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        Idle?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            HorizontalMovement();
            VerticalMovement();
        }
        else
        {
            ApplyGravity();
        }
    }

    // apply horizontal movement on x and z axes- going forwards/backwards and sideways
    void HorizontalMovement()
    {
        // check if player is on a ground layer
        isGrounded = Physics.CheckSphere(characterControllerColl.transform.position + new Vector3(0, colliderYPadding, 0), 0.1f, groundLayer);

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

            // if already moving, check whether player is running or sprinting
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
        if (isGrounded && Input.GetButton("Jump") && !isAttacking)
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

    void ApplyGravity()
    {
        playerVerticalVelocity.y += gravity * Time.deltaTime;

        characterController.Move(playerVerticalVelocity * Time.deltaTime);
    }

    // event calls to animators
    // invoke running/sprinting when player is moving while grounded
    void CheckIfStartedMoving()
    {
        if (isGrounded)
        {
            if (!isMoving && !isAttacking)
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
        if (isGrounded && isMoving && !isAttacking)
        {
            Idle?.Invoke();
        }
        isMoving = false;
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
        isLanding = true;

        if (!isMoving)
        {
            Landed?.Invoke();
        }
        else
            isLanding = false;
    }

    public bool CheckIfStartedAttacking(int skillNum)
    {
        switch (skillNum)
        {
            case 0:
                if (isGrounded && !isLanding)
                {
                    isAttacking = true;
                    canMove = false;

                    ForceImpulse?.Invoke();

                    return true;
                }
                break;

            default:
                break;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(characterControllerColl.transform.position + new Vector3(0, colliderYPadding, 0), 0.1f);
    }
}
