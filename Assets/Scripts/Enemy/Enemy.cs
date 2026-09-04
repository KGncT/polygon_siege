using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyData;

    private Transform target; // player
    private float currentHealth;
    private float lastAttackTime;
    private System.Action<Enemy> returnToPool;

    [SerializeField] private Image healthBarImage;

    private SpriteRenderer spriteRenderer; // 2.5D / billboard sprite kullanıyorsan
    [SerializeField] private Animator animator;
    private bool isWalking => Vector3.Distance(transform.position, target.position) > enemyData.attackRange;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // EnemySpawner tarafından spawn edilirken çağrılır
    public void Init(EnemyDataSO enemyData, Transform playerTarget, System.Action<Enemy> returnCallback)
    {
        this.enemyData = enemyData;
        target = playerTarget;
        returnToPool = returnCallback;

        currentHealth = this.enemyData.maxHealth;
        transform.localScale = Vector3.one * this.enemyData.size;

        if (spriteRenderer != null && this.enemyData.sprite != null)
            spriteRenderer.sprite = this.enemyData.sprite;

        lastAttackTime = -this.enemyData.attackCooldown;
    }

    private void OnEnable()
    {
        // Pool'dan tekrar aktif edildiğinde health resetlensin
        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
            // AudioManager.Instance.Play(enemyData.audio.roar);        
        }
    }

    private void Update()
    {
        animator.SetBool("isWalking", isWalking);
        
        if (target == null || enemyData == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > enemyData.attackRange)
        {            
            Vector3 dir = GetDirectionToPlayer();
            Walk(dir);
        }
        else
        {
            TryAttack();
        }
    }

    private Vector3 GetDirectionToPlayer()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }

    private void Walk(Vector3 direction)
    {
        // İlerleme ve Dönüş
        transform.position += direction * enemyData.speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < enemyData.attackCooldown) return;

        lastAttackTime = Time.time;
        
        animator.SetTrigger("bite");
        AudioManager.Instance.Play(enemyData.audio.bite);
        target.GetComponent<Player>()?.TakeDamage((int)enemyData.damage);
    }

    public void TakeDamage(float amount)
    {
        Debug.Log(currentHealth);
        currentHealth -= amount;
        UpdateHealthbar();
        if (currentHealth <= 0f)
        {
            ScoreManager.Instance.AddScore(enemyData.scoreValue);
            Instantiate(enemyData.deathEffectPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance.Play(enemyData.audio.death);
            Die();
        }
    }

    private void Die()
    {
        // ScoreManager.Instance?.AddScore(data.scoreValue);
        returnToPool?.Invoke(this);
    }

    // Mermiyle temas (PlayerProjectile'daki "Enemy" tag kontrolüyle uyumlu)
    public int ScoreValue => enemyData.scoreValue;

    private void UpdateHealthbar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / enemyData.maxHealth;
        }
    }
}