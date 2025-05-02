using UnityEngine;
using UnityEngine.AI; // Використовується для руху по NavMesh

public class TankAI : MonoBehaviour
{
    public float health = 150f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public float shootingRange = 15f;
    public Transform turretTransform; // Трансформ башти танка для прицілювання
    public Transform playerTransform; // Посилання на гравця
    public TankShooting tankShooting; // Посилання на скрипт стрільби танка
    private Rigidbody rb;
    private NavMeshAgent navMeshAgent; // Для руху по NavMesh

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tankShooting = GetComponent<TankShooting>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = rotationSpeed * 360f;
            navMeshAgent.stoppingDistance = shootingRange - 2f;
            navMeshAgent.isStopped = true; // Спочатку стоїмо
        }

        if (playerTransform == null)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= shootingRange)
            {
                // В межах дистанції стрільби - зупиняємося та стріляємо
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                RotateTowardsPlayer();
                if (tankShooting != null)
                {
                    tankShooting.Shoot();
                }
            }
            else
            {
                // Поза межами дистанції стрільби - переслідуємо гравця
                if (navMeshAgent != null)
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(playerTransform.position);
                }
                else
                {
                    // Простий рух без NavMesh
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    rb.linearVelocity = direction * moveSpeed; // Використовуємо velocity для Rigidbody
                    RotateTowardsPlayer();
                }
            }
        }
    }

    void RotateTowardsPlayer()
    {
        // Поворот корпусу танка
        Vector3 targetDirection = (playerTransform.position - transform.position).normalized;
        targetDirection.y = 0f; // Ігноруємо вертикальну складову
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Поворот башти (якщо є)
        if (turretTransform != null)
        {
            Vector3 turretTargetDirection = (playerTransform.position - turretTransform.position).normalized;
            turretTargetDirection.y = 0f;
            Quaternion turretTargetRotation = Quaternion.LookRotation(turretTargetDirection);
            turretTransform.rotation = Quaternion.Slerp(turretTransform.rotation, turretTargetRotation, Time.deltaTime * rotationSpeed);
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
        EnemySpawner.EnemyDefeated();
        Destroy(gameObject);
    }
}