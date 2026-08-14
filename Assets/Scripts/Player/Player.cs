using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public int health = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Oyuncu öldüğünde yapılacak işlemler
        Debug.Log("Player has died!");
        // Örneğin, sahneyi yeniden yükleyebilir veya oyun sonu ekranına geçiş yapabilirsiniz.
    }
}
