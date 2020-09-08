using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    int currentHealth = 50;

    public void Heal(int amount)
    {
        currentHealth += amount;
        Debug.Log(gameObject.name + " has healed " + amount);
    }
}
