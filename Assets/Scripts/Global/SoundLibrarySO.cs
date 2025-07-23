using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    FootStep,
    Land,
    Hit,
    SwordSwing,
    GaintSwordSwing
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{

    [System.Serializable]
    public class SoundGroup
    {
        [Tooltip("Type of sound this group represents.")]
        public SoundType soundType;

        [Tooltip("Audio clips for this sound type.")]
        public AudioClip[] clips;
    }

    [SerializeField, Header("Sound Groups")]
    private List<SoundGroup> soundGroups = new List<SoundGroup>();

    private Dictionary<SoundType, AudioClip[]> clipDictionary = new Dictionary<SoundType, AudioClip[]>();

    /// <summary>
    /// Rebuild dictionary after any changes in the inspector (Editor only).
    /// </summary>
#if UNITY_EDITOR
    private void OnValidate()
    {
        BuildClipDictionary();
    }
#endif

    public void BuildClipDictionary()
    {
        clipDictionary.Clear();
        foreach (var group in soundGroups)
        {
            if (!clipDictionary.ContainsKey(group.soundType))
            {
                clipDictionary[group.soundType] = group.clips;
            }
        }
    }

    public AudioClip GetRandomClip(SoundType type)
    {
        if (clipDictionary.TryGetValue(type, out var clips) && clips.Length > 0)
        {
            int index = Random.Range(0, clips.Length);
            return clips[index];
        }

        Debug.LogWarning($"No clips found for SoundType '{type}'.", this);
        return null;
    }

}

