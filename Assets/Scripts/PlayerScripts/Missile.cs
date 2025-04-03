using UnityEngine;

public class Missile : MonoBehaviour
{
    public int damage = 20; // Daño que inflige el misil
    public float explosionRadius = 3f; // Radio del área de daño
    public float lifespan = 5f; // Tiempo de vida antes de autodestruirse
    public Animator animator; // Animator para la animación de explosión

    private bool hasExploded = false; // Evitar múltiples explosiones

    private void OnEnable()
    {
        // Restablecer el estado del misil cuando se active desde el pool
        hasExploded = false;
        Invoke(nameof(ReturnToPool), lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            hasExploded = true;

            // Detener la velocidad del misil
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Reproducir animación de explosión
            if (animator != null)
            {
                animator.SetTrigger("Explode");
            }

            // Infligir daño en área
            Explode();

            // Devolver el misil al pool después de un breve tiempo
            Invoke(nameof(ReturnToPool), 0.5f);
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        Debug.Log("Misil explotó e infligió daño en área.");
    }

    private void ReturnToPool()
    {
        CancelInvoke(); // Cancelar cualquier invocación pendiente
        gameObject.SetActive(false); // Desactivar el objeto
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de la explosión en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}