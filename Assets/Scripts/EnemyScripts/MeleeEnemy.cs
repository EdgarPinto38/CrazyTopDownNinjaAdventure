using UnityEngine;
using System.Collections;

public class MeleeEnemy : EnemyBase
{
    public int meleeDamage = 10; // Da�o del ataque cuerpo a cuerpo
    public float attackRange = 0.3f; // Distancia m�nima para atacar al jugador
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    private float lastAttackTime = 0f;

    private Transform playerTransform; // Referencia al transform del jugador
   
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

       

        // Obtener el componente Animator
        animator = GetComponent<Animator>();

        // Obtener la referencia al WaveManager y actualizar los atributos
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            speed = waveManager.GetCurrentEnemySpeed(); // Actualizar velocidad
            meleeDamage = waveManager.GetCurrentEnemyDamage(); // Actualizar da�o

            // Mostrar informaci�n en la consola sobre la velocidad y el da�o del enemigo
            Debug.Log($"[MeleeEnemy] Velocidad: {speed}, Da�o: {meleeDamage}");
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            if (!isAttacking) // Solo moverse si no est� atacando
            {
                MoveTowardsPlayer();
            }

            // Verificar si el jugador est� dentro del rango de ataque
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
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

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

        
    }

   

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            isAttacking = true; // Activar estado de ataque

            // Activar animaci�n de ataque
            if (animator != null)
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false); // Detener la animaci�n de movimiento
            }

            // Infligir da�o al jugador
            PlayerMovement player = playerTransform.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(meleeDamage);
                Debug.Log($"[MeleeEnemy] Atac� al jugador con da�o: {meleeDamage}");
                lastAttackTime = Time.time; // Actualizar el tiempo del �ltimo ataque
            }

            // Salir del estado de ataque despu�s de un breve periodo
            StartCoroutine(ExitAttackState());
        }
    }

    private IEnumerator ExitAttackState()
    {
        yield return new WaitForSeconds(0.5f); // Duraci�n personalizada del ataque
        isAttacking = false;

        // Regresar a la animaci�n base
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsMoving", true);
        }
    }
}