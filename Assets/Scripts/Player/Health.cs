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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        inGameHUD.UpdateHP();
        thirdPersonMovement.TakeDamageKnockback();

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {

    }
}
