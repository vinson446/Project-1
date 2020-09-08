using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : Ability
{
    int healAmount = 25;

    public override void Use(Transform origin, Transform target)
    {
        if (target == null)
        {
            return;
        }

        Debug.Log("Cast Cure on " + target.gameObject.name);

        target.GetComponent<Health>()?.Heal(healAmount);
    }
}
