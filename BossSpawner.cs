using UnityEngine;
using System.Collections;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Prefabs")]
    public GameObject boss1Prefab;
    public GameObject boss2Prefab;
    public GameObject boss3Prefab;

    [Header("Spawn Position")]
    public Vector3 bossSpawnPosition = new Vector3(0f, 6f, 0f);

    [Header("Spawn Conditions")]
    public int boss1ScoreThreshold = 20;
    public float nextBossDelay = 2.0f;

    [Header("References")]
    public EnemySpawner enemySpawner;

    [Header("State")]
    public bool bossAlive = false;

    private enum BossPhase
    {
        None,
        Boss1Active,
        Boss1Done,
        Boss2Active,
        Boss2Done,
        Boss3Active,
        Finished
    }

    private BossPhase currentPhase = BossPhase.None;
    private bool waitingNextBoss = false;
    private GameObject currentBoss = null;

    void Start()
    {
        if (enemySpawner == null)
        {
            enemySpawner = Object.FindFirstObjectByType<EnemySpawner>();
        }
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        if (!bossAlive &&
            !waitingNextBoss &&
            currentPhase == BossPhase.None &&
            GameManager.instance.score >= boss1ScoreThreshold)
        {
            SpawnBoss1();
        }
    }

    void SpawnBoss1()
    {
        if (boss1Prefab == null)
        {
            Debug.LogError("BossSpawner: boss1Prefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        DestroyCurrentBossIfNeeded();

        currentBoss = Instantiate(boss1Prefab, bossSpawnPosition, Quaternion.identity);
        SetupSpawnedBoss(currentBoss);

        bossAlive = true;
        waitingNextBoss = false;
        currentPhase = BossPhase.Boss1Active;

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawnEnabled(false);
        }
    }

    void SpawnBoss2()
    {
        if (boss2Prefab == null)
        {
            Debug.LogError("BossSpawner: boss2Prefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        DestroyCurrentBossIfNeeded();

        currentBoss = Instantiate(boss2Prefab, bossSpawnPosition, Quaternion.identity);
        SetupSpawnedBoss(currentBoss);

        bossAlive = true;
        waitingNextBoss = false;
        currentPhase = BossPhase.Boss2Active;

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawnEnabled(false);
        }
    }

    void SpawnBoss3()
    {
        if (boss3Prefab == null)
        {
            Debug.LogError("BossSpawner: boss3Prefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        DestroyCurrentBossIfNeeded();

        currentBoss = Instantiate(boss3Prefab, bossSpawnPosition, Quaternion.identity);
        SetupSpawnedBoss(currentBoss);

        bossAlive = true;
        waitingNextBoss = false;
        currentPhase = BossPhase.Boss3Active;

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawnEnabled(false);
        }
    }

    void SetupSpawnedBoss(GameObject bossObj)
    {
        if (bossObj == null) return;

        BossLife life = bossObj.GetComponent<BossLife>();
        if (life != null)
        {
            life.SetBossSpawner(this);
        }
    }

    void DestroyCurrentBossIfNeeded()
    {
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }
    }

    public void OnBossDefeated()
    {
        bossAlive = false;
        currentBoss = null;

        if (currentPhase == BossPhase.Boss1Active)
        {
            currentPhase = BossPhase.Boss1Done;
            StartCoroutine(SpawnBoss2AfterDelay());
        }
        else if (currentPhase == BossPhase.Boss2Active)
        {
            currentPhase = BossPhase.Boss2Done;
            StartCoroutine(SpawnBoss3AfterDelay());
        }
        else if (currentPhase == BossPhase.Boss3Active)
        {
            currentPhase = BossPhase.Finished;

            if (enemySpawner != null)
            {
                enemySpawner.SetSpawnEnabled(false);
            }
        }
    }

    IEnumerator SpawnBoss2AfterDelay()
    {
        waitingNextBoss = true;
        yield return new WaitForSeconds(nextBossDelay);
        waitingNextBoss = false;
        SpawnBoss2();
    }

    IEnumerator SpawnBoss3AfterDelay()
    {
        waitingNextBoss = true;
        yield return new WaitForSeconds(nextBossDelay);
        waitingNextBoss = false;
        SpawnBoss3();
    }
}