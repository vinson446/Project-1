using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    [SerializeField] Health hp;
    [SerializeField] Slider hpBar;

    // Start is called before the first frame update
    void Start()
    {
        hpBar.maxValue = hp.MaxHealth;
        hpBar.value = hp.CurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHP()
    {
        hpBar.value = hp.CurrentHealth;
        if (hpBar.value <= 0)
        {
            hpBar.gameObject.SetActive(false);
        }
    }
}
