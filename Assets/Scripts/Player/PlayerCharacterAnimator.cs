using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacterAnimator : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement thirdPersonMovement = null;

    const string IdleState = "Idle";
    const string RunState = "Running";
    const string JumpState = "Jumping";
    const string FallState = "Falling";
    const string LandedState = "Landed";
    const string SprintState = "Sprinting";

    Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnIdle()
    {
        animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnStartRunning()
    {
        animator.CrossFadeInFixedTime(RunState, 0.2f);
    }

    void OnJumping()
    {
        animator.CrossFadeInFixedTime(JumpState, 0.2f);
    }

    void OnFalling()
    {
        animator.CrossFadeInFixedTime(FallState, 0.2f);
    }

    void OnLanded()
    {
        animator.CrossFadeInFixedTime(LandedState, 0.2f);
        StartCoroutine(TransitionFromLandedToIdle());
    }

    void OnSprinting()
    {
        animator.CrossFadeInFixedTime(SprintState, 0.2f);
    }

    IEnumerator TransitionFromLandedToIdle()
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[4].length);

        if (!thirdPersonMovement.isMoving)
            animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnStartSprinting()
    {
        animator.CrossFadeInFixedTime(SprintState, 0.2f);
    }

    private void OnEnable()
    {
        thirdPersonMovement.Idle += OnIdle;
        thirdPersonMovement.StartRunning += OnStartRunning;
        thirdPersonMovement.Jumping += OnJumping;
        thirdPersonMovement.Falling += OnFalling;
        thirdPersonMovement.Landed += OnLanded;
        thirdPersonMovement.StartSprinting += OnStartSprinting;
    }

    private void OnDisable()
    {
        thirdPersonMovement.Idle -= OnIdle;
        thirdPersonMovement.StartRunning -= OnStartRunning;
        thirdPersonMovement.Jumping -= OnJumping;
        thirdPersonMovement.Falling -= OnFalling;
        thirdPersonMovement.Landed -= OnLanded;
        thirdPersonMovement.StartSprinting -= OnStartSprinting;
    }
}
