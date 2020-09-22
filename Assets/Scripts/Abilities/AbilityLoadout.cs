using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoadout : MonoBehaviour
{
    [SerializeField] bool isReady = true;
    float cooldown;

    public Ability EquippedAbility
    {
        get;
        private set;
    }

    ThirdPersonMovement thirdPersonMovement;

    private void Awake()
    {
        thirdPersonMovement = GetComponentInParent<ThirdPersonMovement>();    
    }

    public void EquipAbility(Ability ability)
    {
        RemoveCurrentAbilityObject();
        CreateNewAbilityObject(ability);

        cooldown = ability.Cooldown;
    }

    public void SpawnVFX()
    {
        if (isReady)
        {
            if (thirdPersonMovement.CheckIfStartedAttacking(0))
            {
                EquippedAbility.SpawnVFX();
            }
        }
    }

    public void ChargeAbility()
    {
        if (isReady)
        {
            EquippedAbility.Charge();
        }
    }

    public void UseEquippedAbility(Transform target)
    {
        if (isReady)
        {
            if (thirdPersonMovement.CheckIfStartedAttacking(1))
            {
                EquippedAbility.Use(this.transform, target);

                isReady = false;
                Invoke("CooldownReset", cooldown);
            }
        }
    }

    void CooldownReset()
    {
        isReady = true;
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
