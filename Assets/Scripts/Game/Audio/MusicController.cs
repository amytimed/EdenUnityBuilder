using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource Source;
    public AudioSource AmbienceSource;

    public AudioClip[] Songs;

    public AudioClip[] Ambiences;

    public bool isPlaying;

    private bool _isPlayingNow;

    private bool _isUpdatedAmbience;

    void Start()
    {

    }

    IEnumerator CheckMusic()
    {
        yield return new WaitForSeconds(Random.Range(10, 120));
        Source.PlayOneShot(GetRandomSong());
        isPlaying = false;
        _isPlayingNow = true;
    }

    private AudioClip GetRandomSong()
    {
        return Songs[Random.Range(0, Songs.Length)];
    }

    void Update()
    {
        if (GameSettings.Music)
        {
            if (_isPlayingNow)
            {
                AmbienceSource.volume = Mathf.MoveTowards(AmbienceSource.volume, 0.0f, 0.05f);
            }
            else
            {
                AmbienceSource.volume = Mathf.MoveTowards(AmbienceSource.volume, 1.0f, 0.05f);
            }

            if (!Source.isPlaying && !isPlaying)
            {
                isPlaying = true;
                _isPlayingNow = false;
                StartCoroutine(CheckMusic());
            }
        }
    }
}