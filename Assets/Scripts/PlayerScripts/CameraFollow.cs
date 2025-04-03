using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public Vector3 offset;  // Ajuste de posición (desfase)

    void Update()
    {
        if (player != null)
        {
            // Seguir al jugador con el desfase configurado
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
        }
    }
}