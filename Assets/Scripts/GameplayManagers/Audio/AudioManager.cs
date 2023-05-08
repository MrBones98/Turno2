using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private int _defaultClipIndex;

    [SerializeField]
    List<Sound> sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    private void Start()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }

    }
    public void PlaySound(string name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Play();
                sounds[i].isPLaying = true;
                return;
            }
        }
        // no sounds with name found
        Debug.LogError("AudioManager: No sound with name /" + name + "/ found in list");
    }
    public void StopSound(string name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Stop();
                return;
            }
        }
        // no sounds with name found
        Debug.LogError("AudioManager: No sound with name /" + name + "/ found in list");
    } // not used in current build but is useful for future reference
    public void StopAllBGM()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].isPLaying == true && !sounds[i].isSFX)        //if bgm is playing and clip isn't a sound effect 
            {
                sounds[i].Stop();                                       // stop playing
            }
        }
    }
    public string GetActiveBGMName()
    {
        string clipName = "no clip";
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].isPLaying == true && !sounds[i].isSFX)        //if bgm is playing and clip isn't a sound effect 
            {
                clipName = sounds[i].name;                              // return name of active clip
                return clipName;
            }
        }
        return clipName;                                                // otherwise return "default"

    }

    [Button, DisableInEditorMode]
    private void DebugPlayFirstSoundInList()
    {
        PlaySound(sounds[0].name);
    }
}


