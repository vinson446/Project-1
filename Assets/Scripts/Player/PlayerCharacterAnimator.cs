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

    [Header("Audio")]
    [SerializeField] AudioClip[] clips;
    AudioSource audioSource;

    [Header("Audio- Movement Settings")]
    [SerializeField] float movingVolume;
    [SerializeField] float movingPitch;
    [SerializeField] float runInterval;
    [SerializeField] float sprintInterval;

    [Header("Audio- Jump Settings")]
    [SerializeField] float jumpVolume;
    [SerializeField] float jumpPitch;

    [Header("Audio- Landed Settings")]
    [SerializeField] float landedVolume;
    [SerializeField] float landedPitch;

    private void Awake()
    {
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    public void OnIdle()
    {
        animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnStartRunning()
    {
        animator.CrossFadeInFixedTime(RunState, 0.1f);

        StopAllCoroutines();
        StartCoroutine(LoopRun(movingVolume, movingPitch, runInterval));
    }

    IEnumerator LoopRun(float volume, float pitch, float interval)
    {
        // wait for thirdPersonMovement.isMoving to turn true
        yield return new WaitForSeconds(0.01f);

        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.clip = clips[0];

        while (thirdPersonMovement.isMoving && !thirdPersonMovement.isDead && !thirdPersonMovement.IsHurt && !thirdPersonMovement.IsAttacking)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.Play();

            yield return new WaitForSeconds(interval);
        }
    }

    void OnJumping()
    {
        animator.CrossFadeInFixedTime(JumpState, 0.2f);

        StopAllCoroutines();
        PlayJump(jumpVolume, jumpPitch);
    }

    void PlayJump(float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.PlayOneShot(clips[1]);
    }

    void OnFalling()
    {
        animator.CrossFadeInFixedTime(FallState, 0.2f);
    }

    void OnLanded()
    {
        animator.CrossFadeInFixedTime(LandedState, 0.2f);

        StopAllCoroutines();
        PlayLanded(landedVolume, landedPitch);

        StartCoroutine(TransitionFromLandedToXAnimation());
    }

    void PlayLanded(float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.PlayOneShot(clips[2]);
    }

    IEnumerator TransitionFromLandedToXAnimation()
    {
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[4].length);
        thirdPersonMovement.IsLanding = false;

        if (thirdPersonMovement.isDead)
            animator.CrossFadeInFixedTime(DieState, 0.2f);
        else if (thirdPersonMovement.IsHurt)
            animator.CrossFadeInFixedTime(HurtState, 0.2f);
        else if (!thirdPersonMovement.isMoving && !thirdPersonMovement.isJumping)
            animator.CrossFadeInFixedTime(IdleState, 0.2f);
    }

    void OnStartSprinting()
    {
        animator.CrossFadeInFixedTime(SprintState, 0.1f);

        StopAllCoroutines();
        StartCoroutine(LoopRun(movingVolume, movingPitch, sprintInterval));
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
