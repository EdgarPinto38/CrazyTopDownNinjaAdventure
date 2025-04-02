using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Daño que inflige el proyectil
    public float lifespan = 5f; // Tiempo de vida del proyectil antes de destruirse automáticamente

    private void Start()
    {
        // Destruir el proyectil después de un tiempo
        Destroy(gameObject, lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto impactado está en la capa "Enemies"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            // Obtener el script del enemigo y aplicar daño
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Proyectil impactó al enemigo.");
            }

            // Destruir el proyectil
            Destroy(gameObject);
        }
    }
}