using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Додаємо простір імен для TextMeshPro

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public GameObject prefab;
        public int count;
        [HideInInspector] public int spawned; // Відстежує, скільки вже заспавнено цього типу
    }

    public EnemySpawnInfo[] enemiesToSpawn; // Масив інформації про ворогів для спавну цим спавнером
    public float spawnInterval = 5f; // Інтервал між спавнами в секундах
    public Transform[] spawnPoints; // Масив точок спавну
    public TMP_Text enemyCounterText; // Посилання на текстовий елемент лічильника
    public string winSceneName = "WinScene"; // Назва сцени перемоги
    public bool isMasterSpawner = false; // Чи цей спавнер головний і контролює загальну кількість живих ворогів

    public static int enemiesAlive = 0; // Статична змінна для відстеження кількості живих ворогів
    private float nextSpawnTime = 0f;
    private static bool canSpawn = true; // Статичний прапорець, щоб контролювати спавн
    private static EnemySpawner instance; // Статичний екземпляр головного спавнера
    private int totalEnemiesToSpawn = 0; // Загальна кількість ворогів для спавну (локально для кожного спавнера)

    void Awake()
    {
        if (isMasterSpawner)
        {
            if (instance == null)
            {
                instance = this;
                enemiesAlive = 0; // Ініціалізуємо лічильник ворогів при створенні головного спавнера
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Обчислюємо загальну кількість ворогів для цього спавнера
        foreach (var enemyInfo in enemiesToSpawn)
        {
            totalEnemiesToSpawn += enemyInfo.count;
        }
    }

    void Start()
    {
        if (isMasterSpawner)
        {
            if (enemyCounterText == null)
            {
                enabled = false;
            }
            UpdateEnemyCounterText(); // Ініціалізуємо текст лічильника
        }
        nextSpawnTime = Time.time + spawnInterval; // Ініціалізуємо час першого спавну
    }

    void Update()
    {
        if (canSpawn)
        {
            SpawnEnemiesFromThisSpawner();
        }

        if (isMasterSpawner && enemiesAlive <= 0 && TotalSpawnedCount() == CalculateTotalEnemiesGlobal())
        {
            canSpawn = false;
            enabled = false;
            LoadWinScene();
        }
    }

    void SpawnEnemiesFromThisSpawner()
    {
        if (spawnPoints.Length > 0)
        {
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnPointIndex];

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i].spawned < enemiesToSpawn[i].count && Time.time >= nextSpawnTime)
                {
                    if (enemiesToSpawn[i].prefab != null)
                    {
                        Instantiate(enemiesToSpawn[i].prefab, spawnPoint.position, spawnPoint.rotation);
                        enemiesToSpawn[i].spawned++;
                        EnemySpawner.EnemySpawned(); // Повідомляємо головний спавнер про створення ворога
                        nextSpawnTime = Time.time + spawnInterval;
                        return; // Спавнили одного ворога за кадр, виходимо
                    }
                    else
                    {
                        enabled = false;
                    }
                }
            }
        }
        else
        {
            enabled = false;
        }
    }

    static public void EnemySpawned()
    {
        if (instance != null)
        {
            enemiesAlive++;
            instance.UpdateEnemyCounterText();
        }
    }

    public static void EnemyDefeated()
    {
        if (instance != null)
        {
            enemiesAlive--;
            instance.UpdateEnemyCounterText();
            if (enemiesAlive <= 0 && instance.TotalSpawnedCount() == instance.CalculateTotalEnemiesGlobal())
            {
                instance.LoadWinScene();
            }
        }
    }

    int TotalSpawnedCount()
    {
        int totalSpawned = 0;
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            foreach (var enemyInfo in spawner.enemiesToSpawn)
            {
                totalSpawned += enemyInfo.spawned;
            }
        }
        return totalSpawned;
    }

    int CalculateTotalEnemiesGlobal()
    {
        int total = 0;
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            foreach (var enemyInfo in spawner.enemiesToSpawn)
            {
                total += enemyInfo.count;
            }
        }
        return total;
    }

    void UpdateEnemyCounterText()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"Enemies Alive: {enemiesAlive}";
        }
    }

    void LoadWinScene()
    {
        SceneManager.LoadScene(winSceneName);
    }
}