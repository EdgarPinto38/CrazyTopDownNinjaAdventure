using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    private Rigidbody2D rb;
    private Vector2 movement;

    public float health = 100f; // Vida del jugador
    public float shield = 50f; // Escudo del jugador
    public float maxHealth = 100f; // Vida m�xima
    public float maxShield = 50f; // Escudo m�ximo

    public float shieldRegenDelay = 5f; // Tiempo sin recibir da�o antes de regenerar escudo
    public float shieldRegenRate = 10f; // Velocidad de regeneraci�n del escudo (por segundo)

    private float lastDamageTime; // Tiempo del �ltimo da�o recibido

    public Image lifeBar; // Barra de vida
    public Image shieldBar; // Barra de escudo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastDamageTime = Time.time; // Inicializar el tiempo del �ltimo da�o

        // Asegurarse de inicializar las barras correctamente
        UpdateUI();
    }

    void Update()
    {
        // Movimiento del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0; // Priorizar movimiento horizontal
        }

        movement = new Vector2(horizontal, vertical);

        // Regenerar escudo si no se recibe da�o
        if (Time.time >= lastDamageTime + shieldRegenDelay)
        {
            RegenerateShield();
        }

        // Actualizar las barras de vida y escudo
        UpdateUI();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        lastDamageTime = Time.time; // Actualizar el tiempo del �ltimo da�o

        if (shield > 0)
        {
            // Primero reducir el da�o al escudo
            shield -= damage;
            if (shield < 0)
            {
                // Si queda da�o restante despu�s de agotar el escudo, aplicarlo a la vida
                health += shield; // `shield` es negativo en este punto
                shield = 0;
            }
        }
        else
        {
            // Si no hay escudo, reducir directamente la vida
            health -= damage;
        }

        Debug.Log("Da�o recibido. Vida restante: " + health + ", Escudo restante: " + shield);

        if (health <= 0)
        {
            Die();
        }
    }

    private void RegenerateShield()
    {
        if (shield < maxShield)
        {
            shield += shieldRegenRate * Time.deltaTime;
            shield = Mathf.Min(shield, maxShield); // Evitar que exceda el m�ximo
        }
    }

    private void UpdateUI()
    {
        lifeBar.fillAmount = health / maxHealth;
        shieldBar.fillAmount = shield / maxShield;
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Aqu� puedes a�adir l�gica para reiniciar el nivel o mostrar Game Over
    }
}