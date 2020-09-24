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

    // Start is called before the first frame update
    void Start()
    {
        hpBar.maxValue = hp.MaxHealth;
        hpBar.value = hp.CurrentHealth;

        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void UpdateHP()
    {
        hpBar.value = hp.CurrentHealth;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void ShowSkillChargeVisually()
    {
        mouseImage.color = skillCDColor;
    }

    public void ShowSkillCooldownVisually()
    {
        skillImage.color = skillCDColor;
    }

    public void UpdateCooldownText(float cd)
    {
        cooldownText.text = (cd + 1).ToString();
    }

    public void ResetSkillVisually()
    {
        skillImage.color = skillNormalColor;
        mouseImage.color = skillNormalColor;
        cooldownText.text = "";
    }
}
