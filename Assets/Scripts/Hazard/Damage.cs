using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] bool doDamage = true;
    [SerializeField] int damage = 5;
    [SerializeField] float cooldown = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerColl")
        {
            Health player = other.gameObject.GetComponentInParent<Health>();

            if (doDamage)
            {
                StartCoroutine(DealDamage(player));
            }
        }
    }

    IEnumerator DealDamage(Health h)
    {
        doDamage = false;
        h.TakeDamage(damage);

        yield return new WaitForSeconds(cooldown);

        doDamage = true;
    }
}
