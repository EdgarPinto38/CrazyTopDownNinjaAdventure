using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10; // Daño base al jugador
    public float lifespan = 5f; // Tiempo de vida del proyectil
    public float projectileSpeed = 8f; // Velocidad fija del proyectil

    private void OnEnable()
    {
        // Actualizar el daño basado en la oleada actual
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            damage = waveManager.GetCurrentEnemyDamage();
        }

        // Restablecer el tiempo de vida cuando el objeto se active desde el pool
        Invoke(nameof(ReturnToPool), lifespan);
    }

    public void Launch(Vector3 targetPosition)
    {
        // Calcular la dirección hacia el objetivo
        Vector2 shootDirection = (targetPosition - transform.position).normalized;

        // Asignar velocidad constante al proyectil
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * projectileSpeed;

        // Ajustar la rotación del proyectil para que apunte hacia la dirección del objetivo
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("Proyectil impactó al jugador y causó daño: " + damage);

            // Devolver el proyectil al pool
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke(); // Cancelar cualquier invocación pendiente
        gameObject.SetActive(false); // Desactivar el objeto
    }
}