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

    [SerializeField] GameObject radiusVFX;
    [SerializeField] float maxRadius;
    [SerializeField] float radiusGrowthFactor;
    GameObject radiusObj;
    public bool hasSpawnedVFX = false;

    [SerializeField] AudioClip[] clips;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void SpawnVFX()
    {
        hasSpawnedVFX = true;
        radiusObj = Instantiate(radiusVFX, transform.position + new Vector3(0, 0.1f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        radiusObj.transform.parent = this.gameObject.transform;
        radiusObj.transform.localScale = new Vector3(range, range, range);
    }

    public override void Charge()
    {   
        if (hasSpawnedVFX && radiusObj.transform.localScale.x < maxRadius)
            radiusObj.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * radiusGrowthFactor;
    }

    public override void Use(Transform origin, Transform target)
    {
        if (hasSpawnedVFX)
            StartCoroutine(DelayUseToMatchAnimation(origin));
    }

    IEnumerator DelayUseToMatchAnimation(Transform origin)
    {
        hasSpawnedVFX = false;
        range = radiusObj.transform.localScale.x;
        Destroy(radiusObj);

        audioSource.PlayOneShot(clips[0]);

        yield return new WaitForSeconds(0.3f);

        range = 5;

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
