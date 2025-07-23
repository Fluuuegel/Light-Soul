using UnityEngine;
using System.Collections.Generic;
using Gadgets.Singleton;

public class GameResourceManager : Singleton<GameResourceManager>
{

    [SerializeField, Header("Resources")] private SoundLibrarySO _soundlibrary;

    private void Awake()
    {
        _soundlibrary.BuildClipDictionary();
    }

    public void PlaySfx(AudioSource audioSource, SoundType sound)
    {
        audioSource.clip = _soundlibrary.GetRandomClip(sound);
        audioSource.Play();
    }

}
