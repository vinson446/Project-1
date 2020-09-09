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
            abilityLoadout.UseEquippedAbility(transform);
        }
    }
}
