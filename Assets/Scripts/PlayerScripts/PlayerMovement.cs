using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para usar TextMeshPro

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    private Rigidbody2D rb; // Referencia al Rigidbody2D
    private Vector2 movement; // Movimiento del jugador

    public Animator animator; // Referencia al Animator

    public float health = 100f; // Vida del jugador
    public float shield = 50f; // Escudo del jugador
    public float maxHealth = 100f; // Vida m�xima
    public float maxShield = 50f; // Escudo m�ximo

    public float shieldRegenDelay = 5f; // Tiempo sin recibir da�o antes de regenerar escudo
    public float shieldRegenRate = 10f; // Velocidad de regeneraci�n del escudo (por segundo)

    private float lastDamageTime; // Tiempo del �ltimo da�o recibido

    public Image lifeBar; // Barra de vida
    public Image shieldBar; // Barra de escudo

    // Panel de Game Over
    public GameObject gameOverPanel;
    public TMP_Text waveText; // Texto para mostrar la oleada alcanzada (TextMeshPro)
    public TMP_Text scoreText; // Texto para mostrar la puntuaci�n (TextMeshPro)

    private WaveManager waveManager; // Referencia al WaveManager

    public TMP_Text coinCounterText; // Texto para mostrar el contador de monedas
    private int coinCount = 0; // Contador de monedas

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastDamageTime = Time.time; // Inicializar el tiempo del �ltimo da�o

        // Inicializar las barras de vida y escudo
        UpdateUI();

        // Ocultar el panel de Game Over al inicio
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Buscar el WaveManager en la escena
        waveManager = FindObjectOfType<WaveManager>();
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

        // Actualizar el par�metro "Speed" en el Animator
        animator.SetFloat("Speed", movement.magnitude);

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
            // Reducir el da�o al escudo primero
            shield -= damage;
            if (shield < 0)
            {
                health += shield; // Da�o restante al jugador
                shield = 0;
            }
        }
        else
        {
            // Reducir directamente la vida si no hay escudo
            health -= damage;
        }


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

    public void CollectCoin()
    {
        coinCount++; // Incrementar el contador de monedas
        UpdateCoinUI(); // Actualizar la UI
    }

    private void UpdateCoinUI()
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = coinCount.ToString();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f; // Detener el tiempo

        // Activar el panel de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Mostrar la oleada alcanzada y la puntuaci�n
            if (waveManager != null)
            {
                waveText.text = "Oleada alcanzada: " + waveManager.GetCurrentWave();
                scoreText.text = "Puntuaci�n: " + waveManager.GetScore();
            }
        }
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        UpdateCoinUI();
    }

    public void RegenerateHealth()
    {
        health = maxHealth;
        UpdateUI();
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        UpdateUI();
    }

    public void IncreaseMaxShield(int amount)
    {
        maxShield += amount;
        UpdateUI();
    }

    public void ReduceShieldRegenDelay(float amount)
    {
        shieldRegenDelay -= amount;
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetMaxShield()
    {
        return maxShield;
    }

    public float GetShieldRegenDelay()
    {
        return shieldRegenDelay;
    }
}