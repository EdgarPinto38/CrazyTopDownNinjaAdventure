using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10; // Da�o al jugador
    public float lifespan = 5f; // Tiempo de vida del proyectil
    public float projectileSpeed = 8f; // Velocidad fija del proyectil

    private void Start()
    {
        Destroy(gameObject, lifespan); // Destruir despu�s de un tiempo
    }

    public void Launch(Vector3 targetPosition)
    {
        // Calcular la direcci�n hacia el objetivo
        Vector2 shootDirection = (targetPosition - transform.position).normalized;

        // Asignar velocidad constante al proyectil
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * projectileSpeed;

        // Ajustar la rotaci�n del proyectil para que apunte hacia la direcci�n del objetivo
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("Proyectil impact� al jugador y caus� da�o.");
            Destroy(gameObject);
        }
    }
}