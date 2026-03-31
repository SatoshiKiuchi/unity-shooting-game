using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour
{
    [Header("HP")]
    public int maxHp = 40;
    public int currentHp;

    [Header("Score")]
    public int scorePoint = 20;

    [Header("Prefabs")]
    public GameObject normalBulletPrefab;
    public GameObject aimedBulletPrefab;
    public GameObject giantBulletPrefab;
    public GameObject explosionPrefab;

    [Header("Attack Loop")]
    public float firstAttackDelay = 1.5f;
    public float attackInterval = 2.0f;
    public float idleTime = 2f;

    [Header("Fan Shot")]
    public int fanBulletCount = 7;
    public float fanAngleMin = -40f;
    public float fanAngleMax = 40f;
    public int fanSweepCount = 3;
    public float fanSweepStep = 10f;
    public float fanShotInterval = 1f;

    [Header("Aimed Shot")]
    public int aimedShotCount = 5;
    public float aimedShotInterval = 0.4f;

    [Header("Giant Shot")]
    public float giantBulletSpeed = 2.5f;

    [Header("Phase 2 (HP 50% below)")]
    public bool usePhase2 = true;
    public int phase2AddFanBulletCount = 2;
    public float phase2AimedShotInterval = 0.25f;
    public float phase2GiantBulletSpeed = 3.5f;

    private bool isDead = false;
    private bool isPhase2 = false;
    private bool isAttacking = false;
    private Transform player;

    void Start()
    {
        currentHp = maxHp;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        StartCoroutine(BossRoutine());
    }

    void Update()
    {
        if (isDead) return;

        if (usePhase2 && !isPhase2 && currentHp <= maxHp / 2)
        {
            isPhase2 = true;
        }
    }

    void OnDisable()
    {
        isDead = true;
        isAttacking = false;
        StopAllCoroutines();
    }

    IEnumerator BossRoutine()
    {
        yield return new WaitForSeconds(firstAttackDelay);

        while (!isDead)
        {
            if (!isAttacking)
            {
                yield return StartCoroutine(FanShotRoutine());
                if (isDead) yield break;

                yield return new WaitForSeconds(attackInterval);
                if (isDead) yield break;

                yield return StartCoroutine(AimedShotRoutine());
                if (isDead) yield break;

                yield return new WaitForSeconds(attackInterval);
                if (isDead) yield break;

                yield return StartCoroutine(GiantShotRoutine());
                if (isDead) yield break;

                yield return new WaitForSeconds(idleTime);
            }

            yield return null;
        }
    }

    IEnumerator FanShotRoutine()
    {
        isAttacking = true;

        int bulletCount = fanBulletCount;
        if (isPhase2)
        {
            bulletCount += phase2AddFanBulletCount;
        }

        for (int sweep = 0; sweep < fanSweepCount; sweep++)
        {
            if (isDead) yield break;

            float offset = (sweep % 2 == 0) ? fanSweepStep : -fanSweepStep;
            FireFanShot(bulletCount, fanAngleMin + offset, fanAngleMax + offset);

            yield return new WaitForSeconds(fanShotInterval);
        }

        isAttacking = false;
    }

    void FireFanShot(int bulletCount, float angleMin, float angleMax)
    {
        if (isDead) return;
        if (normalBulletPrefab == null) return;

        if (bulletCount <= 1)
        {
            SpawnBullet(normalBulletPrefab, Vector2.down);
            return;
        }

        for (int i = 0; i < bulletCount; i++)
        {
            float t = (float)i / (bulletCount - 1);
            float angle = Mathf.Lerp(angleMin, angleMax, t);
            Vector2 dir = RotateVector(Vector2.down, angle);
            SpawnBullet(normalBulletPrefab, dir);
        }
    }

    IEnumerator AimedShotRoutine()
    {
        isAttacking = true;

        float interval = isPhase2 ? phase2AimedShotInterval : aimedShotInterval;

        for (int i = 0; i < aimedShotCount; i++)
        {
            if (isDead) yield break;

            FireAimedShot();
            yield return new WaitForSeconds(interval);
        }

        isAttacking = false;
    }

    void FireAimedShot()
    {
        if (isDead) return;
        if (aimedBulletPrefab == null || player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        SpawnBullet(aimedBulletPrefab, dir);
    }

    IEnumerator GiantShotRoutine()
    {
        isAttacking = true;

        if (!isDead && giantBulletPrefab != null)
        {
            float speed = isPhase2 ? phase2GiantBulletSpeed : giantBulletSpeed;

            GameObject bulletObj = Instantiate(
                giantBulletPrefab,
                transform.position,
                Quaternion.identity
            );

            BossBullet bullet = bulletObj.GetComponent<BossBullet>();
            if (bullet != null)
            {
                bullet.SetDirection(Vector2.down);
                bullet.speed = speed;
            }
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    void SpawnBullet(GameObject prefab, Vector2 direction)
    {
        if (isDead) return;
        if (prefab == null) return;

        GameObject bulletObj = Instantiate(prefab, transform.position, Quaternion.identity);

        BossBullet bullet = bulletObj.GetComponent<BossBullet>();
        if (bullet != null)
        {
            bullet.SetDirection(direction);
        }
    }

    Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float x = v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad);
        float y = v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad);
        return new Vector2(x, y).normalized;
    }
}