using UnityEngine;
using System.Collections;

public class MeleeEnemy : EnemyBase
{
    public int meleeDamage = 10; // Daño del ataque cuerpo a cuerpo
    public float attackRange = 1f; // Distancia mínima para atacar al jugador
    public float moveSpeed = 2f; // Velocidad de movimiento del enemigo

    private Transform playerTransform; // Referencia al transform del jugador
    private SpriteRenderer spriteRenderer; // Componente para manejar el sprite
    private Color originalColor; // Color original del sprite
    public float flashDuration = 0.2f; // Duración del color rojo

    private void Start()
    {
        // Buscar al jugador
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Obtener el componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Guardar el color original del sprite
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Moverse hacia el jugador
            MoveTowardsPlayer();

            // Verificar si el jugador está dentro del rango de ataque
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                Attack();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calcular dirección hacia el jugador
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Moverse en dirección al jugador
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // Cambiar el color del sprite a rojo
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        // Cambiar el color a rojo
        spriteRenderer.color = Color.red;

        // Esperar durante el flash
        yield return new WaitForSeconds(flashDuration);

        // Restaurar el color original
        spriteRenderer.color = originalColor;
    }

    private void Attack()
    {
        // Infligir daño al jugador
        PlayerMovement player = playerTransform.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.TakeDamage(meleeDamage);
        }

       
    }
}