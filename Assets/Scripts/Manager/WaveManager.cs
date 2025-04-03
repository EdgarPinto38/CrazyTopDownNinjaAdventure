using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject explodingEnemyPrefab;

    [Header("Wave Settings")]
    public int baseEnemyCount = 5;
    public float spawnInterval = 1f;

    [Header("Difficulty Scaling")]
    public float baseEnemySpeed = 3f;        // Velocidad base de los enemigos
    public float speedIncreasePerWave = 0.2f; // Aumento de velocidad por oleada
    public int baseEnemyDamage = 10;         // Daño base de los enemigos
    public int damageIncreasePerWave = 5;    // Aumento de daño por oleada

    [Header("UI Elements")]
    public GameObject wavePanel;
    public GameObject nextWaveButton;

    [Header("Spawn Area")]
    public Vector2 spawnAreaCenter;
    public Vector2 spawnAreaSize;

    [Header("UI Text Fields")]
    public TMP_Text waveText;
    public TMP_Text scoreText;
    public TMP_Text coinText;
    public TMP_Text lifeText;
    public TMP_Text shieldText;
    public TMP_Text regenTimeText;
    public TMP_Text missilesText;
    public TMP_Text shopMessage;

    [Header("Player References")]
    public PlayerMovement player;
    public PlayerShooting playerShooting;

    private int waveNumber = 0;
    private int totalEnemiesInWave;
    private int enemiesRemaining;
    private int score = 0;

    // Variables para seguimiento de dificultad actual
    private float currentEnemySpeed;
    private int currentEnemyDamage;

    void Start()
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(false);
        }
        StartNextWave();
    }

    void StartNextWave()
    {
        waveNumber++;
        totalEnemiesInWave = baseEnemyCount + waveNumber; // Aumenta un enemigo por oleada
        enemiesRemaining = totalEnemiesInWave;

        // Actualizar la dificultad basada en el número de oleada
        UpdateDifficulty();

        // Limpiar el escenario antes de comenzar la nueva oleada
        ClearEnemies();
        Time.timeScale = 1f;

        // Mostrar información en la consola
        Debug.Log($"[Oleada {waveNumber}] Preparando oleada...");
        Debug.Log($"- Velocidad de enemigos: {currentEnemySpeed}");
        Debug.Log($"- Daño de enemigos: {currentEnemyDamage}");
        Debug.Log($"- Total de enemigos: {totalEnemiesInWave}");

        // Iniciar una corutina con un retraso antes de spawnear enemigos
        StartCoroutine(DelayedSpawn());
    }

    void UpdateDifficulty()
    {
        // Calcular la velocidad y daño actuales basados en el número de oleada
        currentEnemySpeed = baseEnemySpeed + ((waveNumber - 1) * speedIncreasePerWave);
        currentEnemyDamage = baseEnemyDamage + ((waveNumber - 1) * damageIncreasePerWave);
    }

    // Corutina para retrasar el spawn de enemigos
    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log($"Comenzando spawn de enemigos para la oleada: {waveNumber}");
        StartCoroutine(SpawnWave());
    }

    // Método para eliminar todos los enemigos del escenario
    void ClearEnemies()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < totalEnemiesInWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab = GetRandomEnemyPrefab();
        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Aplicar atributos escalados al enemigo
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            // Modificar la velocidad y el daño del enemigo según la oleada actual
            enemyBase.speed = currentEnemySpeed;
            enemyBase.damage = currentEnemyDamage;

            // Asociar el evento de destrucción del enemigo
            enemyBase.OnDestroyEvent += OnEnemyDestroyed;
        }
    }

    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;
        score++;

        if (enemiesRemaining <= 0)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(true);         
        }

        Time.timeScale = 0f;
        UpdateWavePanel();
    }

    void UpdateWavePanel()
    {
        if (waveText != null) waveText.text = $"Oleada: {waveNumber}";
        if (scoreText != null) scoreText.text = $"Puntuación: {score}";
        if (coinText != null) coinText.text = $"Monedas: {player.GetCoinCount()}";
        // Actualizar estadísticas del jugador
        if (lifeText != null) lifeText.text = $"Vida: {player.GetCurrentHealth()} / {player.GetMaxHealth()}";
        if (shieldText != null) shieldText.text = $"Escudo Máximo: {player.GetMaxShield()}";
        if (regenTimeText != null) regenTimeText.text = $"Tiempo de Regeneración: {player.GetShieldRegenDelay().ToString("F1")} segundos";
        if (missilesText != null) missilesText.text = $"Misiles: {playerShooting.GetCurrentMissiles()} / {playerShooting.GetMaxMissiles()}";
    }

    // Método para pasar a la siguiente oleada al presionar el botón
    public void OnNextWaveButtonClicked()
    {
        wavePanel.SetActive(false);
        StartNextWave();
    }

    // Opciones de la tienda
    public void BuyRegenerateHealth()
    {
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
            player.RegenerateHealth();
            shopMessage.text = "Vida regenerada.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    public void BuyIncreaseMaxHealth()
    {
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
            player.IncreaseMaxHealth(20);
            shopMessage.text = "Vida máxima aumentada.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    public void BuyIncreaseMaxShield()
    {
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
            player.IncreaseMaxShield(20);
            shopMessage.text = "Escudo máximo aumentado.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    public void BuyRefillMissiles()
    {
        if (player.GetCoinCount() >= 2)
        {
            player.SpendCoins(2); // Costo: 2 monedas
            playerShooting.RefillMissiles();
            shopMessage.text = "Misiles recargados.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    public void BuyIncreaseMaxMissiles()
    {
        if (player.GetCoinCount() >= 2)
        {
            player.SpendCoins(2); // Costo: 2 monedas
            playerShooting.IncreaseMaxMissiles(2);
            shopMessage.text = "Capacidad de misiles aumentada.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    public void BuyReduceShieldRegenTime()
    {
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
            player.ReduceShieldRegenDelay(1f);
            shopMessage.text = "Tiempo de regeneración de escudo reducido.";
            UpdateWavePanel(); // Actualizar la interfaz de usuario
        }
        else
        {
            shopMessage.text = "No tienes suficientes monedas.";
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0: return meleeEnemyPrefab;
            case 1: return rangedEnemyPrefab;
            case 2: return explodingEnemyPrefab;
            default: return meleeEnemyPrefab;
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        return new Vector2(randomX, randomY);
    }

    // Métodos para obtener valores actuales (podrían ser usados por otras clases)
    public int GetScore()
    {
        return score;
    }

    public int GetCurrentWave()
    {
        return waveNumber;
    }

    // Métodos para obtener los valores de dificultad actual (para que los enemigos puedan consultarlos)
    public float GetCurrentEnemySpeed()
    {
        return currentEnemySpeed;
    }

    public int GetCurrentEnemyDamage()
    {
        return currentEnemyDamage;
    }

    private void OnDrawGizmosSelected()
    {
        // Establecer el color del gizmo
        Gizmos.color = Color.green;
        // Dibujar el área de spawn como un recuadro
        Vector3 center = new Vector3(spawnAreaCenter.x, spawnAreaCenter.y, 0f); // Convertir a Vector3
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f);       // Convertir a Vector3
        Gizmos.DrawWireCube(center, size);
    }
}