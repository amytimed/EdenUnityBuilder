using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public string ControllerName = "Sounds1";

    public AudioClip[] Sounds;

    private AudioSource _source;

    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (GameSettings.Sounds)
        {
            _source.PlayOneShot(GetRandomSound());
        }
    }

    public void PlaySound(int index)
    {
        if (GameSettings.Sounds)
        {
            if (index > 0 && index <= Sounds.Length)
            {
                _source.PlayOneShot(Sounds[index]);
            }
        }
    }

    public void PlaySoundClip(AudioClip clip)
    {
        if (GameSettings.Sounds)
        {
            _source.PlayOneShot(clip);
        }
    }

    private AudioClip GetRandomSound()
    {
        return Sounds[Random.Range(0, Sounds.Length)];
    }

}