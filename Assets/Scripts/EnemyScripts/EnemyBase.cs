using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health = 100; // Vida del enemigo
    public float speed = 3f; // Velocidad de movimiento
    protected Transform target; // Referencia al objetivo (jugador)

    public GameObject coinPrefab; // Prefab de la moneda
    public float dropChance = 0.4f; // Probabilidad de soltar una moneda (40%)

    // Referencia al WaveManager
    private WaveManager waveManager;

    // Daño que este enemigo inflige al jugador
    public int damage = 10;

    // Evento para notificar al WaveManager cuando el enemigo es destruido
    public delegate void EnemyDestroyed();
    public event EnemyDestroyed OnDestroyEvent;

    private void Start()
    {
        // Encontrar al jugador por referencia directa al componente PlayerMovement
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            target = player.transform;
        }

        // Obtener la referencia al WaveManager
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            // Actualizar la velocidad y el daño basados en la dificultad actual
            speed = waveManager.GetCurrentEnemySpeed();
            damage = waveManager.GetCurrentEnemyDamage();
        }
    }

    public virtual void Move()
    {
        // Movimiento básico hacia el objetivo
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // Lógica para soltar una moneda
        if (Random.value <= dropChance)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        OnDestroyEvent?.Invoke(); // Notificar al WaveManager      
        Destroy(gameObject);
    }
}