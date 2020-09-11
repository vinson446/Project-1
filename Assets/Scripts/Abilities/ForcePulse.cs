using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePulse : Ability
{
    [Header("ForcePulse Settings")]
    [SerializeField] float force;
    [SerializeField] float range;
    [SerializeField] float upwardsPush;
    [SerializeField] float delay;

    [Header("VFX and SFX")]
    [SerializeField] GameObject[] VFX;
    [SerializeField] AudioClip[] clips;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Use(Transform origin, Transform target)
    {
        StartCoroutine(DelayUseToMatchAnimation(origin));
    }

    IEnumerator DelayUseToMatchAnimation(Transform origin)
    {
        audioSource.PlayOneShot(clips[0]);

        yield return new WaitForSeconds(0.3f);

        foreach (GameObject o in VFX)
        {
            GameObject vfx = Instantiate(o, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            ParticleSystem p = vfx.GetComponent<ParticleSystem>();
            p.Play();
        }

        Collider[] coll = Physics.OverlapSphere(origin.position, range);

        foreach (Collider c in coll)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, origin.position, range, upwardsPush);
            }

            Box b = c.GetComponent<Box>();
            if (b != null)
            {
                b.PlaySFX();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
