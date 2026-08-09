using UnityEngine;

[System.Serializable]
public class WeaponAudioSet
{
    public SoundDataSO shoot;
    public SoundDataSO reload;
    public SoundDataSO empty;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Stats")]
    public float fireRate;
    public PlayerProjectile projectilePrefab;
    // ...diğer weapon özellikleri

    [Header("Audio")]
    public WeaponAudioSet audio;
}