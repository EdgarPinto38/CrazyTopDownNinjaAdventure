using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Da�o que inflige el proyectil
    public float lifespan = 5f; // Tiempo de vida del proyectil antes de destruirse autom�ticamente

    public ParticleSystem impactParticles; // Sistema de part�culas para el impacto

    private void OnEnable()
    {
        // Restablecer el tiempo de vida cuando el objeto se active desde el pool
        Invoke(nameof(ReturnToPool), lifespan);
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

                // Activar el sistema de part�culas en la posici�n del impacto
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
        CancelInvoke(); // Cancelar cualquier invocaci�n pendiente
        gameObject.SetActive(false); // Desactivar el objeto
    }
}