using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDataSO", menuName = "Weapons/ProjectileDataSO")]
public class ProjectileDataSO : ScriptableObject
{
    public float speed;
    public float damage;
    public GameObject muzzleEffectPrefab;
    public GameObject impactEffectPrefab;
    public GameObject trailEffectPrefab;
}
