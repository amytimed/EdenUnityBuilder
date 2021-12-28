using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSoundController : MonoBehaviour
{

    public TypeAudioBlock[] Actions;

    private AudioSource _source;

    [Serializable]
    public class TypeAudioBlock
    {
        public Constants.Blocks TypeBlock;
        public Constants.Action TypeActon;
        public AudioClip[] Clips;
    }

    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayRandomSound(Constants.Blocks typeblock, Constants.Action typeAction)
    {
        if (GameSettings.Sounds)
        {
            _source.PlayOneShot(GetRandomClipFromSector(typeblock, typeAction));
        }
    }

    private AudioClip GetRandomClipFromSector(Constants.Blocks typeblock, Constants.Action typeAction)
    {
        for (int i = 0; i < Actions.Length; i++)
        {
            if (Actions[i].TypeBlock == typeblock && Actions[i].TypeActon == typeAction)
            {
                return Actions[i].Clips[UnityEngine.Random.Range(0, Actions[i].Clips.Length)];
            }
        }
        return null;
    }

    void Update()
    {

    }
}
