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
    [SerializeField] float VFXGrowthRate;
    public float VFXScale;
    float startingVFXScale = 0.5f;

    [SerializeField] GameObject radiusVFX;
    [SerializeField] float maxRadius;
    [SerializeField] float radiusGrowthFactor;
    float startingRadius = 3;
    GameObject radiusObj;
    bool hasSpawnedVFX = false;

    AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        VFXScale = startingVFXScale;
        startingRadius = range;
    }

    public override void SpawnVFX()
    {
        hasSpawnedVFX = true;

        radiusObj = Instantiate(radiusVFX, transform.position + new Vector3(0, 0.1f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        radiusObj.transform.parent = this.gameObject.transform;
        radiusObj.transform.localScale = new Vector3(startingRadius, startingRadius, startingRadius);
    }

    public override void Charge()
    {   
        if (hasSpawnedVFX && radiusObj.transform.localScale.x < maxRadius / 2 - 1)
        {
            radiusObj.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * radiusGrowthFactor;
            range = radiusObj.transform.localScale.x * 2;

            VFXScale += Time.deltaTime * VFXGrowthRate;
        }
    }

    public override void Use(Transform origin, Transform target)
    {
        if (hasSpawnedVFX)
            StartCoroutine(DelayUseToMatchAnimation(origin));
    }

    IEnumerator DelayUseToMatchAnimation(Transform origin)
    {
        hasSpawnedVFX = false;

        Destroy(radiusObj);

        audioSource.PlayOneShot(clips[0]);

        yield return new WaitForSeconds(0.3f);

        foreach (GameObject o in VFX)
        {
            GameObject vfx = Instantiate(o, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            vfx.transform.localScale = new Vector3(VFXScale, VFXScale, VFXScale);

            ParticleSystem p = vfx.GetComponent<ParticleSystem>();
            p.Play();
        }

        Collider[] coll = Physics.OverlapSphere(origin.position, range);

        foreach (Collider c in coll)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null && c.gameObject.tag != "Spike")
            {
                rb.AddExplosionForce(force, origin.position, range, upwardsPush);
            }

            Box b = c.GetComponent<Box>();
            if (b != null)
            {
                b.PlaySFX();
            }
        }

        range = startingRadius;
        VFXScale = startingVFXScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
