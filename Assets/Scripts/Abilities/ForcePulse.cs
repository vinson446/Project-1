﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePulse : Ability
{
    [Header("ForcePulse Settings")]
    [SerializeField] float force;
    [SerializeField] float range;
    [SerializeField] float upwardsPush;
    [SerializeField] float delay;

    private void Awake()
    {

    }

    public override void Use(Transform origin, Transform target)
    {
        StartCoroutine(DelayUseToMatchAnimation(origin));
    }

    IEnumerator DelayUseToMatchAnimation(Transform origin)
    {
        yield return new WaitForSeconds(0.3f);

        Collider[] coll = Physics.OverlapSphere(origin.position, range);

        foreach (Collider c in coll)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, origin.position, range, upwardsPush);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}