using UnityEngine;

public class ExplodingEnemy : EnemyBase
{
    public int explosionDamage = 20; // Daño de la explosión
    public float explosionRadius = 3f; // Radio de la explosión
    public float explosionRange = 1.5f; // Distancia para detenerse antes de explotar
    public Animator explosionAnimator; // Animator para la animación de explosión
    private bool isExploding = false; // Para evitar múltiples explosiones
    private Transform playerTransform; // Referencia al transform del jugador
    private WaveManager waveManager; // Referencia al WaveManager

    private void Start()
    {
        // Buscar al jugador
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Buscar el WaveManager
        waveManager = FindObjectOfType<WaveManager>();
    }

    private void Update()
    {
        if (!isExploding && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > explosionRange)
            {
                // Moverse hacia el jugador si está fuera del rango de explosión
                MoveTowardsPlayer();
            }
            else
            {
                // Detenerse y explotar si está dentro del rango
                StartExplosion();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    private void StartExplosion()
    {
        if (!isExploding)
        {
            isExploding = true;
            if (explosionAnimator != null)
            {
                explosionAnimator.SetTrigger("Explode"); // Activar la animación
            }

            // Causa daño tras un breve retraso
            Invoke("Explode", 0.5f); // Ajustar tiempo según la duración de la animación
        }
    }

    private void Explode()
    {
        // Infligir daño a todos los objetos en el radio de explosión
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            PlayerMovement player = collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }
        }

        // Notificar al WaveManager que este enemigo ha sido eliminado
        if (waveManager != null)
        {
            waveManager.OnEnemyDestroyed();
        }

        // Destruir al enemigo después de la explosión
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el rango de explosión y el radio de daño
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRange); // Rango para detenerse
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Radio de daño
    }
}