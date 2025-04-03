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
        EnemyProjectile enemyProjectile = bullet.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            // Lanzar el proyectil hacia la posición del jugador
            enemyProjectile.Launch(target.position);
            Debug.Log("Proyectil disparado correctamente.");
        }
        else
        {
            Debug.LogError("El proyectil no tiene el componente EnemyProjectile.");
        }
    }
}