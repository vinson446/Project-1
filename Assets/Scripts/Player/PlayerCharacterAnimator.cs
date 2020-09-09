using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacterAnimator : MonoBehaviour
{
    ThirdPersonMovement thirdPersonMovement = null;

    const string IdleState = "Idle";
    const string RunState = "Running";
    const string JumpState = "Jumping";
    const string FallState = "Falling";
    const string LandedState = "Landed";
    const string SprintState = "Sprinting";

    const string ForceImpulseState = "ForceImpulse";

    Animator animator = null;

    private void Awake()
    {
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
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

    IEnumerator TransitionFromLandedToIdle()
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[4].length);
        thirdPersonMovement.IsLanding = false;

        if (!thirdPersonMovement.isMoving && !thirdPersonMovement.isJumping)
            animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnSprinting()
    {
        animator.CrossFadeInFixedTime(SprintState, 0.2f);
    }

    void OnForceImpulse()
    {
        animator.CrossFadeInFixedTime(ForceImpulseState, 0.2f);
        StartCoroutine(TransitionFromAttackToGroundedAnimation(6));
    }

    IEnumerator TransitionFromAttackToGroundedAnimation(int index)
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[index].length);

        thirdPersonMovement.IsAttacking = false;
        thirdPersonMovement.CanMove = true;

        if (thirdPersonMovement.isSprinting)
            OnSprinting();
        else if (thirdPersonMovement.isMoving)
            OnStartRunning();
        else if (!thirdPersonMovement.isMoving)
            OnIdle();

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

        thirdPersonMovement.ForceImpulse += OnForceImpulse;
    }

    private void OnDisable()
    {
        thirdPersonMovement.Idle -= OnIdle;
        thirdPersonMovement.StartRunning -= OnStartRunning;
        thirdPersonMovement.Jumping -= OnJumping;
        thirdPersonMovement.Falling -= OnFalling;
        thirdPersonMovement.Landed -= OnLanded;
        thirdPersonMovement.StartSprinting -= OnStartSprinting;

        thirdPersonMovement.ForceImpulse -= OnForceImpulse;
    }
}
