using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10; // Daño al jugador
    public float lifespan = 5f; // Tiempo de vida del proyectil

    private void Start()
    {
        Destroy(gameObject, lifespan); // Destruir después de un tiempo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("Proyectil impactó al jugador y causó daño.");
            Destroy(gameObject);
        }
    }
}