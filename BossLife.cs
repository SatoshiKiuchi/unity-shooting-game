using UnityEngine;

public class BossLife : MonoBehaviour
{
    public int maxHp = 40;
    public int currentHp = 40;
    public int scorePoint = 20;

    public GameObject explosionPrefab;
    public GameObject damagePopupPrefab;

    private BossSpawner bossSpawner;
    private Boss1 boss1;
    private Boss2 boss2;
    private Boss3 boss3;
    private bool isDead = false;

    void Awake()
    {
        boss1 = GetComponent<Boss1>();
        boss2 = GetComponent<Boss2>();
        boss3 = GetComponent<Boss3>();
    }

    void Start()
    {
        if (boss1 != null)
        {
            maxHp = boss1.maxHp;
            currentHp = boss1.maxHp;
            scorePoint = boss1.scorePoint;
            if (boss1.explosionPrefab != null) explosionPrefab = boss1.explosionPrefab;
            boss1.currentHp = currentHp;
        }
        else if (boss2 != null)
        {
            maxHp = boss2.maxHp;
            currentHp = boss2.maxHp;
            scorePoint = boss2.scorePoint;
            if (boss2.explosionPrefab != null) explosionPrefab = boss2.explosionPrefab;
            boss2.currentHp = currentHp;
        }
        else if (boss3 != null)
        {
            maxHp = boss3.maxHp;
            currentHp = boss3.maxHp;
            scorePoint = boss3.scorePoint;
            if (boss3.explosionPrefab != null) explosionPrefab = boss3.explosionPrefab;
            boss3.currentHp = currentHp;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (boss1 != null) boss1.currentHp = currentHp;
        if (boss2 != null) boss2.currentHp = currentHp;
        if (boss3 != null) boss3.currentHp = currentHp;
    }

    public void SetBossSpawner(BossSpawner spawner)
    {
        bossSpawner = spawner;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        ShowDamage(damage);

        currentHp -= damage;

        if (boss1 != null) boss1.currentHp = currentHp;
        if (boss2 != null) boss2.currentHp = currentHp;
        if (boss3 != null) boss3.currentHp = currentHp;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (boss1 != null)
        {
            boss1.currentHp = 0;
            boss1.enabled = false;
        }

        if (boss2 != null)
        {
            boss2.currentHp = 0;
            boss2.enabled = false;
        }

        if (boss3 != null)
        {
            boss3.currentHp = 0;
            boss3.enabled = false;
        }

        // 念のため、このオブジェクト上の全コルーチン停止
        StopAllCoroutines();

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject b in bullets)
        {
            Destroy(b);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scorePoint);
            GameManager.instance.PlaySE(GameManager.instance.enemyExplosionSE);
        }

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in sprites)
        {
            sr.enabled = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // 次のBossが出る前に残骸が残らないようにする
        Destroy(gameObject, 0.05f);

        UpgradeManager manager = UpgradeManager.instance;
        if (manager == null)
        {
            manager = Object.FindFirstObjectByType<UpgradeManager>();
        }

        if (manager != null)
        {
            manager.OpenUpgradePanel();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("BossLife: UpgradeManager が見つかりません。");
        }
    }

    void ShowDamage(int damage)
    {
        if (damagePopupPrefab == null) return;

        GameObject popup = Instantiate(
            damagePopupPrefab,
            transform.position,
            Quaternion.identity
        );

        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            popupScript.Setup(damage);
        }
    }
}