using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.CollectCoin(); // Llamar al método del jugador para recoger la moneda
            Destroy(gameObject); // Destruir la moneda
            Debug.Log("Moneda recogida!");
        }
    }
}