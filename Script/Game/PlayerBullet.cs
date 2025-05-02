using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public string shooterTag;
    public int damage;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (shooterTag == "Player" && other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                Destroy(gameObject); // Знищуємо кулю після влучання
            }
            else
            {
                TankAI tankAI = other.GetComponent<TankAI>();
                if (tankAI != null)
                {
                    tankAI.TakeDamage(damage);
                    Destroy(gameObject); // Знищуємо кулю після влучання
                }
            }
        }
        else
        {
            Destroy(gameObject); // Знищуємо кулю при зіткненні з будь-яким іншим об'єктом
        }
    }
}