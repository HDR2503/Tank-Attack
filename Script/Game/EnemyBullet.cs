using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 30;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerArtilleryController playerArtilleryController = other.GetComponent<PlayerArtilleryController>();
            if (playerArtilleryController != null)
            {
                playerArtilleryController.TakeDamage(damage);
                Destroy(gameObject); // Знищуємо кулю після влучання
            }
        }
        else if (!other.CompareTag("Bullet")) // Знищуємо кулю при зіткненні з іншими об'єктами, крім інших куль
        {
            Destroy(gameObject);
        }
    }
}