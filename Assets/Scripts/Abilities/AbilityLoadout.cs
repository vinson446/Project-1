using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoadout : MonoBehaviour
{
    [SerializeField] bool isReady = true;
    float cooldown;

    InGameHUD inGameHUD;

    public Ability EquippedAbility
    {
        get;
        private set;
    }

    ThirdPersonMovement thirdPersonMovement;

    private void Awake()
    {
        thirdPersonMovement = GetComponentInParent<ThirdPersonMovement>();
        inGameHUD = FindObjectOfType<InGameHUD>();
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

                inGameHUD.ShowSkillCooldownVisually();
                StartCoroutine(CooldownReset(cooldown));
            }
        }
    }

    IEnumerator CooldownReset(float cd)
    {
        while (cd > 0)
        {
            cd -= 1;
            inGameHUD.UpdateCooldownText(cd);

            yield return new WaitForSeconds(1);
        }

        isReady = true;

        inGameHUD.ResetSkillVisually();
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
