using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
// this is a data structure that holds all the variables and methods for the instance 
// that the Audio manager creates
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loopSound = false;
    public bool isSFX = false;
    [HideInInspector]
    public bool isPLaying = false;

    [SerializeField]
    private AudioMixerGroup _mixer;

    private AudioSource _source;

    public void SetSource(AudioSource source)
    {
        _source = source;
        _source.clip = clip;
        _source.loop = loopSound;
        _source.playOnAwake = false;
        _source.outputAudioMixerGroup = _mixer;
    }
    public void Play()
    {                             // if the random range is 0 then this line defaults to 1
        _source.volume = volume * (1 + UnityEngine.Random.Range(-randomVolume / 2f, randomVolume / 2f));
        _source.pitch = pitch * (1 + UnityEngine.Random.Range(-randomPitch / 2f, randomPitch / 2f));
        _source.Play();
        isPLaying = true;    // used by GetActiveBGMName
    }
    public void Stop()
    {
        _source.Stop();
        isPLaying = false;
    }
}
