using UnityEngine;

public class ExplodingEnemy : EnemyBase
{
    public int explosionDamage = 20; // Daño de la explosión
    public float explosionRadius = 3f; // Radio de la explosión

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Explota al entrar en contacto con cualquier objeto
        Explode();
    }

    void Explode()
    {
        // Infligir daño a objetos cercanos dentro del radio de explosión
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            // Verificar si el objeto tiene un componente "Player"
            var player = collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }

            // Opcional: Daño a otros objetos con scripts que puedan recibir daño
            /*var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }*/
        }

        // Destruirse a sí mismo tras la explosión
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de la explosión en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}