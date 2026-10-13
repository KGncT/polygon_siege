using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxRange = 15f;
    [SerializeField] private float damage = 10f;

    private float currentSpeed;
    private float currentRange;
    private float currentDamage;
    private int pierceLeft;
    private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();

    private Vector3 spawnPosition;
    private System.Action<PlayerProjectile> returnToPool;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // PlayerShooting spawn ederken bu metodu çağırır
    public void Init(System.Action<PlayerProjectile> returnCallback, float damageMultiplier = 1f, float speedMultiplier = 1f, float rangeMultiplier = 1f, int pierce = 0)
    {
        returnToPool = returnCallback;
        spawnPosition = transform.position;

        currentDamage = damage * damageMultiplier;
        currentSpeed = speed * speedMultiplier;
        currentRange = maxRange * rangeMultiplier;
        pierceLeft = pierce;
        hitColliders.Clear();

        rb.linearVelocity = transform.forward * speed;
    }

    private void OnEnable()
    {
        spawnPosition = transform.position;
    }

    private void Update()
    {
        float traveled = Vector3.Distance(spawnPosition, transform.position);
        if (traveled >= currentRange)
        {
            ReturnSelf();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            //Pierce logic
            if (!hitColliders.Add(other.collider))
            {
                rb.linearVelocity = transform.forward * currentSpeed;
                return;
            }

            other.collider.GetComponent<Enemy>()?.TakeDamage(currentDamage);

            if(pierceLeft > 0)
            {
                pierceLeft--;
                rb.linearVelocity = transform.forward * currentSpeed;
                return;
            }
        } 

        ReturnSelf();
    }

    private void ReturnSelf()
    {
        rb.linearVelocity = Vector3.zero;
        returnToPool?.Invoke(this);
    }
}