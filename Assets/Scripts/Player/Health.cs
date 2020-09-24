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

    private void Awake()
    {
        inGameHUD = FindObjectOfType<InGameHUD>();
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
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
        }
        else
        {
            thirdPersonMovement.CheckIfStartedHurt();
        }
    }

    public void Kill()
    {

    }
}
