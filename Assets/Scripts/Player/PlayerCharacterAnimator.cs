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
    const string HurtState = "Hurt";
    const string DieState = "Die";
    const string ChargeState = "Charge";

    Animator animator = null;

    private void Awake()
    {
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            print(animator.runtimeAnimatorController.animationClips[i].name + " " + i);
        }
    }

    public void OnIdle()
    {
        animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnStartRunning()
    {
        animator.CrossFadeInFixedTime(RunState, 0.1f);
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

    void OnStartSprinting()
    {
        animator.CrossFadeInFixedTime(SprintState, 0.1f);
    }

    void OnForceImpulse()
    {
        animator.CrossFadeInFixedTime(ForceImpulseState, 0.2f);
        StartCoroutine(TransitionFromAttackToXAnimation());
    }

    IEnumerator TransitionFromAttackToXAnimation()
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[5].length);

        thirdPersonMovement.IsAttacking = false;
        thirdPersonMovement.CanMove = true;

        if (thirdPersonMovement.isSprinting)
            OnStartSprinting();
        else if (thirdPersonMovement.isMoving)
            OnStartRunning();
        else if (!thirdPersonMovement.isMoving)
            OnIdle();
    }

    void OnHurt()
    {
        animator.CrossFadeInFixedTime(HurtState, 0.3f);
        StartCoroutine(TransitionFromHurtToXAnimation());
    }

    IEnumerator TransitionFromHurtToXAnimation()
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[7].length * 0.9f);

        thirdPersonMovement.IsHurt = false;

        if (thirdPersonMovement.isSprinting)
            OnStartSprinting();
        else if (thirdPersonMovement.isMoving)
            OnStartRunning();
        else if (!thirdPersonMovement.isMoving)
            OnIdle();
    }

    void OnDie()
    {
        animator.CrossFadeInFixedTime(DieState, 0.3f);
    }

    void OnCharge()
    {
        animator.CrossFadeInFixedTime(ChargeState, 0.2f);
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
        thirdPersonMovement.Hurt += OnHurt;
        thirdPersonMovement.Die += OnDie;
        thirdPersonMovement.Charge += OnCharge;
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
        thirdPersonMovement.Hurt -= OnHurt;
        thirdPersonMovement.Die -= OnDie;
        thirdPersonMovement.Charge -= OnCharge;
    }
}
