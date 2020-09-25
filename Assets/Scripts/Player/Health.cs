using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] int currentHealth;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    [SerializeField] int maxHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    InGameHUD inGameHUD;
    ThirdPersonMovement thirdPersonMovement;
    PlaySound soundManager;

    private void Awake()
    {
        inGameHUD = FindObjectOfType<InGameHUD>();

        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        soundManager = GetComponent<PlaySound>();
    }

    private void Start()
    {
        currentHealth = maxHealth;    
    }

    public void TakeDamage(int damage, Transform enemy)
    {
        currentHealth -= damage;

        inGameHUD.UpdateHP();
        thirdPersonMovement.TakeDamageKnockback(enemy);

        if (currentHealth <= 0)
        {
            Kill();

            thirdPersonMovement.CheckIfStartedDead();

            StopAllCoroutines();
            soundManager.PlaySFXOneShot(0, 0.75f, 0.85f);
        }
        else
        {
            thirdPersonMovement.CheckIfStartedHurt();

            StopAllCoroutines();
            soundManager.PlaySFXOneShot(0, 0.75f, 0.9f);
        }
    }

    public void Kill()
    {

    }
}
