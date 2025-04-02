using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10; // Da�o al jugador
    public float lifespan = 5f; // Tiempo de vida del proyectil

    private void Start()
    {
        Destroy(gameObject, lifespan); // Destruir despu�s de un tiempo
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