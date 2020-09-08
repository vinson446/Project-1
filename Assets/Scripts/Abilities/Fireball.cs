using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Ability
{
    [SerializeField] GameObject projectile = null;

    int rank = 1;

    public override void Use(Transform origin, Transform target)
    {
        GameObject proj = Instantiate(projectile, origin.position, origin.rotation);

        if (target != null)
        {
            proj.transform.LookAt(target);
        }

        Destroy(proj, 3.5f);
    }
}
