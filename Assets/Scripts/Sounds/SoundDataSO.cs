using UnityEngine;

public enum AudioType { SFX, Music}

[CreateAssetMenu(fileName = "SoundDataSO", menuName = "ScriptableObjects/SoundDataSo")]
public class SoundDataSO : ScriptableObject
{
    public AudioClip clip;
    public AudioType type;
    [Range(0f, 1f)] public float volume = 1f;
}
