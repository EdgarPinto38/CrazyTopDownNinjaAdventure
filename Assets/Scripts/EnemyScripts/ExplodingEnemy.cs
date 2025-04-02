using UnityEngine;

public class ExplodingEnemy : EnemyBase
{
    public int explosionDamage = 20; // Da�o de la explosi�n
    public float explosionRadius = 3f; // Radio de la explosi�n

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Explota al entrar en contacto con cualquier objeto
        Explode();
    }

    void Explode()
    {
        // Infligir da�o a objetos cercanos dentro del radio de explosi�n
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            // Verificar si el objeto tiene un componente "Player"
            var player = collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }

            // Opcional: Da�o a otros objetos con scripts que puedan recibir da�o
            /*var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }*/
        }

        // Destruirse a s� mismo tras la explosi�n
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de la explosi�n en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}