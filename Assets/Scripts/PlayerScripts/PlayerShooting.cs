using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab para las balas
    public GameObject missilePrefab; // Prefab para los misiles
    public float bulletSpeed = 10f; // Velocidad de las balas
    public float missileSpeed = 8f; // Velocidad de los misiles
    public int maxMissiles = 2; // Máximo de misiles disponibles al inicio
    private int currentMissiles; // Misiles disponibles actualmente
    public float bulletFireRate = 2f; // Tiempo entre cada bala disparada
    public float missileFireRate = 5f; // Tiempo entre cada misil disparado
    private float lastBulletTime; // Última vez que se disparó una bala
    private float lastMissileTime; // Última vez que se disparó un misil
    private bool isShootingBullet = false; // Verifica si se está manteniendo presionado el clic izquierdo
    private bool isShootingMissile = false; // Verifica si se está manteniendo presionado el clic derecho

    public TMP_Text missilesText;

    void Start()
    {
        // Inicializar la cantidad de misiles disponibles y los tiempos de disparo
        currentMissiles = maxMissiles;
        lastBulletTime = -bulletFireRate; // Permitir disparar balas inmediatamente al inicio
        lastMissileTime = -missileFireRate; // Permitir disparar misiles inmediatamente al inicio

        missilesText.text = currentMissiles.ToString();
    }

    void Update()
    {
        // Detectar clic izquierdo para iniciar el disparo automático de balas
        if (Input.GetMouseButtonDown(0)) // 0 = Clic izquierdo
        {
            if (!isShootingBullet) // Si no se está disparando balas ya
            {
                isShootingBullet = true;
                StartCoroutine(AutoShootBullets());
            }
        }

        // Detectar si se deja de presionar el clic izquierdo
        if (Input.GetMouseButtonUp(0)) // Soltar clic izquierdo
        {
            isShootingBullet = false; // Detener el disparo automático de balas
        }

        // Detectar clic derecho para iniciar el disparo automático de misiles
        if (Input.GetMouseButtonDown(1)) // 1 = Clic derecho
        {
            if (!isShootingMissile && currentMissiles > 0) // Si no se está disparando misiles ya y hay misiles disponibles
            {
                isShootingMissile = true;
                StartCoroutine(AutoShootMissiles());
            }
        }

        // Detectar si se deja de presionar el clic derecho
        if (Input.GetMouseButtonUp(1)) // Soltar clic derecho
        {
            isShootingMissile = false; // Detener el disparo automático de misiles
        }
    }

    IEnumerator AutoShootBullets()
    {
        while (isShootingBullet) // Mientras se mantenga presionado el clic izquierdo
        {
            if (Time.time >= lastBulletTime + bulletFireRate) // Verificar si ha pasado suficiente tiempo
            {
                Shoot(bulletPrefab, bulletSpeed); // Disparar una bala
                lastBulletTime = Time.time; // Actualizar la última vez que se disparó
            }
            yield return null; // Esperar al siguiente fotograma
        }
    }

    IEnumerator AutoShootMissiles()
    {
        while (isShootingMissile && currentMissiles > 0) // Mientras se mantenga presionado el clic derecho y haya misiles disponibles
        {
            if (Time.time >= lastMissileTime + missileFireRate) // Verificar si ha pasado suficiente tiempo
            {
                Shoot(missilePrefab, missileSpeed); // Disparar un misil
                lastMissileTime = Time.time; // Actualizar la última vez que se disparó
                currentMissiles--; // Reducir la cantidad de misiles disponibles
                missilesText.text = currentMissiles.ToString();
            }
            yield return null; // Esperar al siguiente fotograma
        }
    }

    void Shoot(GameObject projectilePrefab, float projectileSpeed)
    {
        // Usar ObjectPoolManager para obtener un proyectil del pool
        GameObject projectile = ObjectPoolManager.Instance.SpawnFromPool(projectilePrefab, transform.position, Quaternion.identity);

        if (projectile == null)
        {
            Debug.LogError("No se pudo obtener un proyectil del pool.");
            return;
        }

        // Calcular la posición del mouse en el mundo y la dirección hacia él
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Aseguramos que la posición Z sea 0 para 2D

        Vector2 shootDirection = (mousePosition - transform.position).normalized; // Dirección normalizada

        // Asignar velocidad fija al proyectil
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * projectileSpeed; // Velocidad constante

        // Ajustar la rotación del proyectil para que apunte hacia el mouse
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void RefillMissiles()
    {
        currentMissiles = maxMissiles;
        missilesText.text = currentMissiles.ToString();
    }

    public void IncreaseMaxMissiles(int amount)
    {
        maxMissiles += amount;
    }

    public int GetCurrentMissiles()
    {
        return currentMissiles;
    }

    public int GetMaxMissiles()
    {
        return maxMissiles;
    }
}