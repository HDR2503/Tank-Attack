using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float health = 100f;
    public float moveSpeed = 3f;
    public float attackRange = 10f;
    public float attackDamage = 10f;
    public float attackInterval = 2f;
    private float nextAttackTime = 0f;
    private Transform target; // Тепер ціль - артилерія-гравець
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        FindPlayerArtillery(); // Шукаємо артилерію-гравця
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = attackRange - 1f;
        }
    }

    void Update()
    {
        if (target == null)
        {
            FindPlayerArtillery(); // Якщо ціль втрачено, знову шукаємо артилерію
            return;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(target.position);

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime && !isAttacking)
            {
                isAttacking = true;
                AttackTarget();
                nextAttackTime = Time.time + attackInterval;
            }
            else if (distanceToTarget > attackRange)
            {
                isAttacking = false;
            }
        }
        else
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime && !isAttacking)
            {
                isAttacking = true;
                AttackTarget();
                nextAttackTime = Time.time + attackInterval;
            }
            else if (distanceToTarget > attackRange)
            {
                isAttacking = false;
            }
        }
    }

    void FindPlayerArtillery()
    {
        GameObject playerArtillery = GameObject.FindGameObjectWithTag("Player"); // Або "PlayerArtillery"
        if (playerArtillery != null)
        {
            target = playerArtillery.transform;
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            PlayerArtilleryController artilleryController = target.GetComponent<PlayerArtilleryController>();
            if (artilleryController != null)
            {
                artilleryController.TakeDamage(attackDamage);
                // Додайте тут логіку анімації атаки ворога
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        EnemySpawner.EnemyDefeated(); // Повідомляємо спавнер про смерть ворога
        Destroy(gameObject);
    }
}