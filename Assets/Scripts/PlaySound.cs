using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // called once
    public void PlaySFXOneShot(int index, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.loop = false;

        audioSource.PlayOneShot(clips[index]);
    }

    // called once
    public void PlaySFXLooping(int index, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.clip = clips[index];
        audioSource.loop = true;

        audioSource.Play();
    }
}
