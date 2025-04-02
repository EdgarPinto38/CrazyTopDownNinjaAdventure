using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public GameObject bulletPrefab; // Prefab del proyectil
    public float shootCooldown = 2f; // Tiempo entre disparos
    public float stopDistance = 5f; // Distancia mínima antes de disparar
    private float lastShootTime = 0f; // Última vez que disparó

    private void Update()
    {
        if (target != null)
        {
            MoveTowardsPlayer(); // Movimiento hacia el jugador
            ShootWhenReady(); // Disparo
        }
    }

    public override void Move()
    {
        // Detenerse al alcanzar la distancia mínima
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void MoveTowardsPlayer()
    {
        Move();
    }

    private void ShootWhenReady()
    {
        if (Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Prefab del proyectil no asignado.");
            return;
        }

        // Instanciar el proyectil
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        if (bullet != null)
        {
            Debug.Log("Proyectil instanciado correctamente: " + bullet.name);
        }
        else
        {
            Debug.LogError("La instancia del proyectil falló.");
        }

        // Configurar velocidad del proyectil
        Vector3 direction = (target.position - transform.position).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 3f; // Velocidad ajustada
        }
    }
}