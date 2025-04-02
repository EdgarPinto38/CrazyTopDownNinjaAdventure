using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Da�o que inflige el proyectil
    public float lifespan = 5f; // Tiempo de vida del proyectil antes de destruirse autom�ticamente

    private void Start()
    {
        // Destruir el proyectil despu�s de un tiempo
        Destroy(gameObject, lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto impactado est� en la capa "Enemies"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            // Obtener el script del enemigo y aplicar da�o
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Proyectil impact� al enemigo.");
            }

            // Destruir el proyectil
            Destroy(gameObject);
        }
    }
}