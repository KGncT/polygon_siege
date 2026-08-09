using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyData;

    private Transform target; // player
    private float currentHealth;
    private float lastAttackTime;
    private System.Action<Enemy> returnToPool;

    [Header("Walk Animasyonu (Seke Seke Yürüyüş)")]
    [SerializeField] private float hopHeight = 0.15f;
    [SerializeField] private float hopFrequency = 6f;
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Image healthBarImage;
    private Vector3 baseLocalPosition;
    private float walkTimer;    

    private SpriteRenderer spriteRenderer; // 2.5D / billboard sprite kullanıyorsan

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (visualRoot == null)
            visualRoot = transform;

        baseLocalPosition = visualRoot.localPosition;
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

        walkTimer = Random.Range(0f, 10f);
    }

    private void OnEnable()
    {
        // Pool'dan tekrar aktif edildiğinde health resetlensin
        if (enemyData != null)
            currentHealth = enemyData.maxHealth;

        if (visualRoot != null)
            visualRoot.localPosition = baseLocalPosition;
    }

    private void Update()
    {
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
            ResetWalkVisual();
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
        // İlerleme
        transform.position += direction * enemyData.speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        // Yürüyüş fazı ilerlet
        walkTimer += Time.deltaTime * hopFrequency;

        // Tek bir sinüs dalgası: hem zıplama hem yaslanma AYNI fazdan türesin
        float wave = Mathf.Sin(walkTimer * Mathf.PI); // -1..1 arası

        // Zıplama: sadece pozitif kısmı kullan (yerden kalkıp inme)
        float hop = Mathf.Abs(wave);
        Vector3 hopOffset = Vector3.up * hop * hopHeight;

        // Yaslanma: wave'in işaretine göre direkt sağ/sol tilt (ekstra Sign çarpımı YOK)
        float tilt = wave * tiltAngle;

        visualRoot.localPosition = baseLocalPosition + hopOffset;
        visualRoot.localRotation = Quaternion.Euler(0f, 0f, tilt);
    }

    private void ResetWalkVisual()
    {
        visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, baseLocalPosition, Time.deltaTime * 10f);
        visualRoot.localRotation = Quaternion.Lerp(visualRoot.localRotation, Quaternion.identity, Time.deltaTime * 10f);
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < enemyData.attackCooldown) return;

        lastAttackTime = Time.time;
        // player'a hasar verme mantığı
        // target.GetComponent<PlayerHealth>()?.TakeDamage(data.damage);
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