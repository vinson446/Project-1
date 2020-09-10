using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] protected float cooldown;
    public float Cooldown
    {
        get => cooldown;
        set => cooldown = value;
    }

    public abstract void Use(Transform origin, Transform target);
}
