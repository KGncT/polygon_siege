using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public int health = 100;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject deathEffectPrefab;

    public int MaxHealth { get; private set;} = 100;


    public void TakeDamage(int amount)
    {
        health -= amount;
        levelManager.DecreaseEnergy(amount);
        if (health <= 0)
        {
            if(levelManager.characterLives > 0)
            {
                levelManager.DecreaseLives(1);
                health = MaxHealth;
                levelUIManager.ResetUIElements();
                Die();
            }
            else
            {
                levelManager.OnGameOver();
            }
        }
    }

    //Card: Heal card selected
    public void Heal(int amount)
    {
        int healed = Mathf.Min(amount, MaxHealth - health);
        if (healed < 0)
            return;

        health += healed;
        levelManager.IncreaseEnergy(healed);
    }

    //Card: Increase max health card selected
    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        health += amount;
        levelManager.IncreaseMaxEnergy(amount);
    }

    private void Die()
    {
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        transform.position += Vector3.up * 20f;
    }
}
