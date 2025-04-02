using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health = 100; // Vida del enemigo
    public float speed = 3f; // Velocidad de movimiento
    protected Transform target; // Referencia al objetivo (jugador)

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
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida restante: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        OnDestroyEvent?.Invoke(); // Notificar al WaveManager
        Debug.Log(gameObject.name + " ha sido destruido.");
        Destroy(gameObject);
    }
}