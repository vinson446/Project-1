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

    // combat animation events
    public event Action ForceImpulse = delegate { };
    public event Action Hurt = delegate { };
    public event Action Die = delegate { };
    public event Action Charge = delegate { };

    [Header("References")]
    [SerializeField] CharacterController characterController;
    Collider characterControllerColl;
    [SerializeField] Transform cam;
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
    [SerializeField] float knockbackBackwardsForce;
    [SerializeField] float knockbackUpwardsForce;
    [SerializeField] float gravity;
    public bool isGrounded { get; private set; }
    Vector3 playerVerticalVelocity;
    Vector3 velocity;

    // animation checks 
    public bool isMoving { get; private set; }
    public bool isJumping { get; private set; }
    bool isFalling;
    bool isLanding;
    public bool IsLanding { get => isLanding; set => isLanding = value; }
    bool isSprinting;
    public bool IsSprinting { get => isSprinting; set => isSprinting = value; }

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    bool isHurt;
    public bool IsHurt { get => isHurt; set => isHurt = value; }
    public bool isDead { get; private set; }

    public bool WasInAir { get; private set; }

    PlayerCharacterAnimator playerCharacterAnimator;

    private void Awake()
    {
        characterControllerColl = characterController.GetComponent<Collider>();

        playerCharacterAnimator = GetComponent<PlayerCharacterAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Idle?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !isHurt && !isDead)
        {
            HorizontalMovement();
            VerticalMovement();
        }
        // knockback
        else if (canMove && (isHurt || isDead))
        {
            characterController.Move((velocity + playerVerticalVelocity) * Time.deltaTime);
        }
        // gravity
        else
        {
            characterController.Move(playerVerticalVelocity * Time.deltaTime);
        }

        ApplyGravity();
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

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = false;
            moveSpeed = runSpeed;
        }
        else
        {
            isSprinting = true;
            moveSpeed = sprintSpeed;
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
                playerVerticalVelocity.y = -2;

            if (isFalling)
                CheckIfStoppedFalling();
        }

        // jump
        if (isGrounded && Input.GetButton("Jump") && !isAttacking)
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
        }

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
    }

    public void TakeDamageKnockback(Transform e)
    {
        transform.LookAt(new Vector3(e.position.x, transform.position.y, e.position.z));
        
        velocity = -transform.forward * knockbackBackwardsForce * -.2f * -9.81f;
        velocity.y = knockbackUpwardsForce;

        StartCoroutine(ReduceKnockbackForceOverTime());
    }

    IEnumerator ReduceKnockbackForceOverTime()
    {
        while (velocity.magnitude > 1)
        {
            velocity /= 2;
            yield return new WaitForSeconds(0.1f);
        }
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
        {
            isLanding = false;

            playerCharacterAnimator.PlayAirborneVFX(1);
        }
    }

    public bool CheckIfStartedAttacking(int skillNum)
    {
        switch (skillNum)
        {
            // force impulse- charge
            case 0:
                if (isGrounded && !isLanding)
                {
                    isAttacking = true;
                    canMove = false;

                    Charge?.Invoke();

                    return true;
                }
                break;
            
            // force impulse- attack
            case 1:
                if (isAttacking)
                {
                    ForceImpulse?.Invoke();

                    return true;
                }
                break;

            default:
                break;
        }

        return false;
    }

    public void CheckIfStartedHurt()
    {
        isHurt = true;

        Hurt?.Invoke();
    }

    public void CheckIfStartedDead()
    {
        isDead = true;

        Die?.Invoke();
        StartCoroutine(StopMovementAfterDeath());
    }

    IEnumerator StopMovementAfterDeath()
    {
        yield return new WaitForSeconds(2);

        canMove = false;
    }
}
