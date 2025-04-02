using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject meleeEnemyPrefab; // Prefab de enemigo cuerpo a cuerpo
    public GameObject rangedEnemyPrefab; // Prefab de enemigo que dispara
    public GameObject explodingEnemyPrefab; // Prefab de enemigo que explota
    public int baseEnemyCount = 5; // N�mero base de enemigos por oleada
    public float spawnInterval = 1f; // Tiempo entre cada aparici�n de enemigos en una oleada

    // Definici�n del �rea de spawn (rectangular)
    public Vector2 spawnAreaCenter; // Centro del �rea de spawn
    public Vector2 spawnAreaSize;   // Tama�o del �rea de spawn (ancho y alto)

    private int waveNumber = 0; // N�mero de la oleada actual

    void Start()
    {
        StartNextWave(); // Iniciar la primera oleada
    }

    void StartNextWave()
    {
        waveNumber++; // Incrementar el n�mero de oleada
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        // Calcular cantidad de enemigos para esta oleada
        int enemyCount = baseEnemyCount + waveNumber * 2; // Incremento de enemigos por oleada

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(); // Hacer aparecer un enemigo
            yield return new WaitForSeconds(spawnInterval); // Esperar tiempo entre apariciones
        }

        // Esperar antes de comenzar la siguiente oleada
        yield return new WaitForSeconds(5f); // Tiempo de descanso entre oleadas
        StartNextWave(); // Comenzar la siguiente oleada
    }

    void SpawnEnemy()
    {
        // Elegir aleatoriamente el tipo de enemigo
        GameObject enemyPrefab = GetRandomEnemyPrefab();

        // Elegir una posici�n aleatoria dentro del �rea delimitada
        Vector2 spawnPosition = GetRandomSpawnPosition();

        // Instanciar el enemigo en la posici�n aleatoria
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    GameObject GetRandomEnemyPrefab()
    {
        // Seleccionar un tipo de enemigo aleatoriamente
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
        // Generar una posici�n aleatoria dentro del �rea rectangular delimitada
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        return new Vector2(randomX, randomY);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el �rea de spawn en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}