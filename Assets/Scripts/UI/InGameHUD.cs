using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHUD : MonoBehaviour
{
    [SerializeField] Health hp;

    [Header("HP UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    [Header("Skill UI")]
    [SerializeField] Image skillImage;
    [SerializeField] Image mouseImage;
    [SerializeField] Color skillNormalColor;
    [SerializeField] Color skillCDColor;
    [SerializeField] TextMeshProUGUI cooldownText;
    float cooldown;

    ThirdPersonMovement thirdPersonMovement;

    private void Awake()
    {
        thirdPersonMovement = FindObjectOfType<ThirdPersonMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hpBar.maxValue = hp.MaxHealth;
        hpBar.value = hp.CurrentHealth;

        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    private void Update()
    {
        if (cooldown > 0)
        {
            ShowSkillChargeColor();
            ShowSkillCooldownColor();
        }
        else
        {
            if (!thirdPersonMovement.isGrounded)
            {
                ShowSkillChargeColor();
                ShowSkillCooldownColor();
            }
            else
            {
                ResetSkillColor();
            }
        }
    }

    public void UpdateHP()
    {
        hpBar.value = hp.CurrentHealth;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void ShowSkillChargeColor()
    {
        mouseImage.color = skillCDColor;
    }

    public void ShowSkillCooldownColor()
    {
        skillImage.color = skillCDColor;
    }

    public void UpdateCooldownText(float cd)
    {
        cooldown = cd + 1;
        cooldownText.text = cooldown.ToString();
    }

    public void ResetSkillColor()
    {
        skillImage.color = skillNormalColor;
        mouseImage.color = skillNormalColor;
    }

    public void ResetSkillCooldown()
    {
        if (cooldown == 1)
            cooldown = 0;

        cooldownText.text = "";
    }
}
