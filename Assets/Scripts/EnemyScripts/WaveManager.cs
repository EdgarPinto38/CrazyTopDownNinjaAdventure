using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject meleeEnemyPrefab; // Prefab de enemigo cuerpo a cuerpo
    public GameObject rangedEnemyPrefab; // Prefab de enemigo que dispara
    public GameObject explodingEnemyPrefab; // Prefab de enemigo que explota

    public int baseEnemyCount = 5; // N�mero base de enemigos por oleada
    public float spawnInterval = 1f; // Tiempo entre cada aparici�n de enemigos

    public GameObject wavePanel; // Panel de pausa entre oleadas
    public GameObject nextWaveButton; // Bot�n para continuar

    public Vector2 spawnAreaCenter; // Centro del �rea de spawn
    public Vector2 spawnAreaSize;   // Tama�o del �rea de spawn

    private int waveNumber = 0; // N�mero de la oleada actual
    private int totalEnemiesInWave; // Total de enemigos que deben aparecer en la oleada
    private int enemiesRemaining; // Enemigos restantes en la oleada
    private bool isPaused = false; // Indica si el juego est� pausado

    private int score = 0; // Puntuaci�n acumulada

    void Start()
    {
        // Asegurar que el panel est� desactivado inicialmente
        if (wavePanel != null)
        {
            wavePanel.SetActive(false);
        }
        StartNextWave(); // Comenzar la primera oleada
    }

    void StartNextWave()
    {
        waveNumber++; // Incrementar el n�mero de oleada
        totalEnemiesInWave = baseEnemyCount + waveNumber; // Calcular el total de enemigos de la oleada
        enemiesRemaining = totalEnemiesInWave; // Inicializar los enemigos restantes
        isPaused = false; // Reanudar el juego
        Time.timeScale = 1f; // Reanudar el tiempo

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < totalEnemiesInWave; i++)
        {
            SpawnEnemy(); // Instanciar un enemigo
            yield return new WaitForSeconds(spawnInterval); // Tiempo entre apariciones
        }
    }

    void SpawnEnemy()
    {
        // Elegir aleatoriamente el tipo de enemigo
        GameObject enemyPrefab = GetRandomEnemyPrefab();

        // Elegir una posici�n aleatoria dentro del �rea delimitada
        Vector2 spawnPosition = GetRandomSpawnPosition();

        // Instanciar el enemigo en la posici�n aleatoria
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Ajustar estad�sticas del enemigo ligeramente
        AdjustEnemyStats(enemy);

        // Registrar la muerte del enemigo para reducir el contador y sumar puntos
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.OnDestroyEvent += OnEnemyDestroyed;
        }
    }

    void OnEnemyDestroyed()
    {
        enemiesRemaining--; // Reducir el contador de enemigos restantes
        score++; // Incrementar la puntuaci�n
        Debug.Log("Enemigos restantes: " + enemiesRemaining);
        Debug.Log("Puntuaci�n actual: " + score);

        if (enemiesRemaining <= 0)
        {
            EndWave();
        }
    }

    void AdjustEnemyStats(GameObject enemy)
    {
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.health += waveNumber * 5; // Incrementar vida
            enemyBase.speed += waveNumber * 0.1f; // Incrementar velocidad
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
        return meleeEnemyPrefab; // Valor por defecto
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        return new Vector2(randomX, randomY);
    }

    void EndWave()
    {
        isPaused = true;
        wavePanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Oleada " + waveNumber + " completada.");
        Debug.Log("Puntuaci�n final: " + score);
        Debug.Log("Panel activado: " + wavePanel.activeSelf);
    }

    public void OnNextWaveButtonClicked()
    {
        // Esconder el panel y comenzar la siguiente oleada
        wavePanel.SetActive(false);
        StartNextWave();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }

    // M�todos para acceder a la puntuaci�n y la oleada actual
    public int GetScore()
    {
        return score; // Devuelve la puntuaci�n acumulada
    }

    public int GetCurrentWave()
    {
        return waveNumber; // Devuelve la oleada actual
    }
}