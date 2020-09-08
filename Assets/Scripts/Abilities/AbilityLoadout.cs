﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoadout : MonoBehaviour
{
    public Ability EquippedAbility
    {
        get;
        private set;
    }

    public void EquipAbility(Ability ability)
    {
        RemoveCurrentAbilityObject();
        CreateNewAbilityObject(ability);
    }

    public void UseEquippedAbility(Transform target)
    {
        EquippedAbility.Use(this.transform, target);
    }

    public void RemoveCurrentAbilityObject()
    {
        foreach(Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void CreateNewAbilityObject(Ability ability)
    {
        EquippedAbility = Instantiate(ability, transform.position, Quaternion.identity);
        EquippedAbility.transform.SetParent(this.transform);
    }
}
