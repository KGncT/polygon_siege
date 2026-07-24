using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "PolygonSiege/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";
    public Sprite sprite;

    [Header("Stats")]
    public float damage = 10f;
    public float speed = 3f;
    public float maxHealth = 20f;
    public int scoreValue = 10;

    [Header("Size")]
    public float size = 1f; // transform.localScale çarpanı olarak kullanılacak

    [Header("Behaviour (opsiyonel)")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
}