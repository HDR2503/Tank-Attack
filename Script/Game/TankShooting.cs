using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireForce = 25f;
    public float fireRate = 2f;
    private float nextFireTime = 0f;

    public void Shoot()
    {
        if (Time.time >= nextFireTime && firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
            }
            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.damage =1000; // Задайте урон для кулі танка
            }
            nextFireTime = Time.time + fireRate;
        }
    }
}