using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Daño que inflige el proyectil
    public float lifespan = 5f; // Tiempo de vida del proyectil antes de destruirse automáticamente

    public ParticleSystem impactParticles; // Sistema de partículas para el impacto

    private void OnEnable()
    {
        // Restablecer el tiempo de vida cuando el objeto se active desde el pool
        Invoke(nameof(ReturnToPool), lifespan);
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

                // Activar el sistema de partículas en la posición del impacto
                if (impactParticles != null)
                {
                    Instantiate(impactParticles, transform.position, Quaternion.identity);
                }
            }

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