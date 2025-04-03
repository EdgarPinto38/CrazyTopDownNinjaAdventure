using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject explodingEnemyPrefab;

    public int baseEnemyCount = 5;
    public float spawnInterval = 1f;

    public GameObject wavePanel;
    public GameObject nextWaveButton;

    public Vector2 spawnAreaCenter;
    public Vector2 spawnAreaSize;

    private int waveNumber = 0;
    private int totalEnemiesInWave;
    private int enemiesRemaining;
    private int score = 0;

    public TMP_Text waveText;
    public TMP_Text scoreText;
    public TMP_Text coinText;
    public TMP_Text lifeText;
    public TMP_Text shieldText;
    public TMP_Text regenTimeText;
    public TMP_Text missilesText;
    public TMP_Text shopMessage;

    public PlayerMovement player;
    public PlayerShooting playerShooting;

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
        waveNumber++; // Incrementar el número de la oleada
        totalEnemiesInWave = baseEnemyCount + waveNumber; // Calcular el total de enemigos para esta oleada
        enemiesRemaining = totalEnemiesInWave; // Reiniciar el contador de enemigos restantes

        // Limpiar el escenario antes de comenzar la nueva oleada
        ClearEnemies();

        Time.timeScale = 1f; // Reanudar el tiempo
        Debug.Log("Preparando oleada: " + waveNumber);

        // Iniciar una corutina con un retraso antes de spawnear enemigos
        StartCoroutine(DelayedSpawn());
    }

    // Corutina para retrasar el spawn de enemigos
    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(3f); // Esperar 3 segundos
        Debug.Log("Comenzando spawn de enemigos para la oleada: " + waveNumber);
        StartCoroutine(SpawnWave());
    }

    // Método para eliminar todos los enemigos del escenario
    void ClearEnemies()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            Destroy(enemy.gameObject); // Destruir el objeto del enemigo
        }

        Debug.Log("Escenario limpiado: todos los enemigos eliminados.");
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

        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.OnDestroyEvent += OnEnemyDestroyed;
        }
    }

    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;
        score++;
        Debug.Log("Enemigos restantes: " + enemiesRemaining);
        Debug.Log("Puntuación actual: " + score);

        if (enemiesRemaining <= 0)
        {
            EndWave();
            Debug.Log("Oleada terminada.");
        }
    }

    void EndWave()
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(true);
            Debug.Log("Panel de oleadas activado.");
        }
        else
        {
            Debug.LogError("El panel de oleadas no está asignado en el inspector.");
        }

        Time.timeScale = 0f;
        UpdateWavePanel();
    }

    void UpdateWavePanel()
    {
        if (waveText != null) waveText.text = "Oleada: " + waveNumber;
        if (scoreText != null) scoreText.text = "Puntuación: " + score;
        if (coinText != null) coinText.text = "Monedas: " + player.GetCoinCount();

        // Actualizar estadísticas del jugador
        if (lifeText != null) lifeText.text = "Vida: " + player.GetCurrentHealth() + " / " + player.GetMaxHealth();
        if (shieldText != null) shieldText.text = "Escudo Máximo: " + player.GetMaxShield();
        if (regenTimeText != null) regenTimeText.text = "Tiempo de Regeneración: " + player.GetShieldRegenDelay().ToString("F1") + " segundos";
        if (missilesText != null) missilesText.text = "Misiles: " + playerShooting.GetCurrentMissiles() + " / " + playerShooting.GetMaxMissiles();
    }


    // Método para pasar a la siguiente oleada al presionar el botón
    public void OnNextWaveButtonClicked()
    {
        Debug.Log("Botón presionado: avanzando a la siguiente oleada.");
        wavePanel.SetActive(false);
        StartNextWave();
    }

    // Opciones de la tienda
    public void BuyRegenerateHealth()
    {
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(10);
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
            player.IncreaseMaxShield(10);
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
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
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
        if (player.GetCoinCount() >= 1)
        {
            player.SpendCoins(1);
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
        }
        return meleeEnemyPrefab;
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        return new Vector2(randomX, randomY);
    }

    public int GetScore()
    {
        return score;
    }

    public int GetCurrentWave()
    {
        return waveNumber;
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