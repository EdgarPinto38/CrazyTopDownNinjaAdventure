using UnityEngine;

public class Missile : MonoBehaviour
{
    public int damage = 20; // Da�o que inflige el misil
    public float explosionRadius = 3f; // Radio del �rea de da�o
    public float lifespan = 5f; // Tiempo de vida antes de autodestruirse
    public Animator animator; // Animator para la animaci�n de explosi�n

    private bool hasExploded = false; // Evitar m�ltiples explosiones

    private void Start()
    {
        Destroy(gameObject, lifespan); // Destruir despu�s de un tiempo si no impacta
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            hasExploded = true; // Marcar que el misil ha explotado

            // Detener la velocidad del misil
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Establecer la velocidad en 0
            }

            // Reproducir animaci�n de explosi�n
            if (animator != null)
            {
                animator.SetTrigger("Explode"); // Activar animaci�n de explosi�n
            }

            // Infligir da�o en �rea
            Explode();

            // Destruir el misil despu�s de un breve tiempo (sincr�nico con la animaci�n)
            Destroy(gameObject, 0.5f); // Ajustar tiempo seg�n la duraci�n de la animaci�n
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
                    enemy.TakeDamage(damage); // Infligir da�o al enemigo dentro del radio
                }
            }
        }

        Debug.Log("Misil explot� e infligi� da�o en �rea.");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de la explosi�n en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}