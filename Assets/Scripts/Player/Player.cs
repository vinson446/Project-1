using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // skills
    AbilityLoadout abilityLoadout;
    [SerializeField] Ability[] abilities;

    private void Awake()
    {
        abilityLoadout = GetComponentInChildren<AbilityLoadout>();
    }

    private void Start()
    {
        abilityLoadout?.EquipAbility(abilities[0]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            abilityLoadout.SpawnVFX();
        }
        else if (Input.GetMouseButton(0))
        {
            abilityLoadout.ChargeAbility();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            abilityLoadout.UseEquippedAbility(transform);
        }
    }
}
