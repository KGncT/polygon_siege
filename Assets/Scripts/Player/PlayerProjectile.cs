using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxRange = 15f;
    [SerializeField] private float damage = 10f;

    private Vector3 spawnPosition;
    private System.Action<PlayerProjectile> returnToPool;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // PlayerShooting spawn ederken bu metodu çağırır
    public void Init(System.Action<PlayerProjectile> returnCallback)
    {
        returnToPool = returnCallback;
        spawnPosition = transform.position;
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnEnable()
    {
        spawnPosition = transform.position;
    }

    private void Update()
    {
        float traveled = Vector3.Distance(spawnPosition, transform.position);
        if (traveled >= maxRange)
        {
            ReturnSelf();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            other.collider.GetComponent<Enemy>()?.TakeDamage(damage);
            ReturnSelf();
        } else {
            ReturnSelf();
        }
    }

    private void ReturnSelf()
    {
        rb.linearVelocity = Vector3.zero;
        returnToPool?.Invoke(this);
    }
}