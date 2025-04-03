using UnityEngine;

public class ExplodingEnemy : EnemyBase
{
    public int explosionDamage = 20; // Da�o de la explosi�n
    public float explosionRadius = 3f; // Radio de la explosi�n
    public float explosionRange = 1.5f; // Distancia para detenerse antes de explotar
    public Animator explosionAnimator; // Animator para la animaci�n de explosi�n
    private bool isExploding = false; // Para evitar m�ltiples explosiones
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
                // Moverse hacia el jugador si est� fuera del rango de explosi�n
                MoveTowardsPlayer();
            }
            else
            {
                // Detenerse y explotar si est� dentro del rango
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
                explosionAnimator.SetTrigger("Explode"); // Activar la animaci�n
            }

            // Causa da�o tras un breve retraso
            Invoke("Explode", 0.5f); // Ajustar tiempo seg�n la duraci�n de la animaci�n
        }
    }

    private void Explode()
    {
        // Infligir da�o a todos los objetos en el radio de explosi�n
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

        // Destruir al enemigo despu�s de la explosi�n
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el rango de explosi�n y el radio de da�o
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRange); // Rango para detenerse
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Radio de da�o
    }
}