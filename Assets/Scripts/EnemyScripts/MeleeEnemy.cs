using UnityEngine;
using System.Collections;

public class MeleeEnemy : EnemyBase
{
    public int meleeDamage = 10; // Daño del ataque cuerpo a cuerpo
    public float attackRange = 0.3f; // Distancia mínima para atacar al jugador
    public float moveSpeed = 2f; // Velocidad de movimiento del enemigo
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    private float lastAttackTime = 0f;

    private Transform playerTransform; // Referencia al transform del jugador
    private SpriteRenderer spriteRenderer; // Componente para manejar el sprite
    private Color originalColor; // Color original del sprite
    public float flashDuration = 0.2f; // Duración del color rojo
    private Animator animator; // Referencia al Animator
    private bool isAttacking = false; // Estado de ataque

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

        // Obtener el componente Animator
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            if (!isAttacking) // Solo moverse si no está atacando
            {
                MoveTowardsPlayer();
            }

            // Verificar si el jugador está dentro del rango de ataque
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                Attack();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // Actualizar estado de movimiento en el Animator
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
                animator.SetBool("IsAttacking", false); // Asegurar que no ataque mientras se mueve
            }
        }
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
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor; // Restaurar el color original
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            isAttacking = true; // Activar estado de ataque

            // Activar animación de ataque
            if (animator != null)
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false); // Detener la animación de movimiento
            }

            // Infligir daño al jugador
            PlayerMovement player = playerTransform.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(meleeDamage);
                lastAttackTime = Time.time; // Actualizar el tiempo del último ataque
            }

            // Salir del estado de ataque después de un breve periodo
            StartCoroutine(ExitAttackState());
        }
    }

    private IEnumerator ExitAttackState()
    {
        yield return new WaitForSeconds(0.5f); // Duración personalizada del ataque
        isAttacking = false;

        // Regresar a la animación base
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsMoving", true);
        }
    }
}