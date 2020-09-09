using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected float cooldown;
    public float Cooldown
    {
        get => cooldown = 5;
        set => cooldown = value;
    }

    public abstract void Use(Transform origin, Transform target);
}
