using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] AbilityLoadout abilityLoadout = null;
    [SerializeField] Ability startingAbility = null;
    [SerializeField] Ability newAbilityToTest = null;

    [SerializeField] Transform testTarget = null;

    public Transform CurrentTarget
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (startingAbility != null)
        {
            abilityLoadout?.EquipAbility(startingAbility);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            abilityLoadout.UseEquippedAbility(CurrentTarget);
        }

        // equip new weapon
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            abilityLoadout.EquipAbility(newAbilityToTest);
        }

        // set a target, for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // target ourselves in this case
            SetTarget(testTarget);
        }
    }
}
