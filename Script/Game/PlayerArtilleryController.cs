using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerArtilleryController : MonoBehaviour
{
    public float health = 100f;
    public float rotationSpeed = 50f;
    public FixedJoystick rotationJoystick;
    public Transform artilleryFirePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public float reloadTime = 3f;
    public Canvas gameUICanvas; // Canvas з основним UI гри
    public string gameOverScene = "GameOverScene";
    private float nextFireTime = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (gameUICanvas != null)
        {
            gameUICanvas.gameObject.SetActive(true); // Вмикаємо UI гри
        }
    }

    void FixedUpdate()
    {
        if (rotationJoystick != null)
        {
            float horizontalRotation = rotationJoystick.Horizontal;
            Vector3 rotation = Vector3.up * horizontalRotation * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(rotation);
        }
    }

    public void Shoot()
    {
        if (Time.time >= nextFireTime && artilleryFirePoint != null && bulletPrefab != null)
        {
            CreateBullet();
            nextFireTime = Time.time + reloadTime;
        }
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, artilleryFirePoint.position, artilleryFirePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(artilleryFirePoint.forward * bulletForce, ForceMode.Impulse);
        }

        // Тепер це куля гравця-артилерії, тому налаштовуємо її відповідно
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterTag = "Player"; // Або "ArtilleryPlayer"
            bulletScript.damage = 200; // Встановіть урон гравця
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Перехід на сцену GameOver
        SceneManager.LoadScene(gameOverScene);
    }
}