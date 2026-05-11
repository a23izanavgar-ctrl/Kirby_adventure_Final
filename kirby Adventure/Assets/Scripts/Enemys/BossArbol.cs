using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BossArbol : Enemycontroller
{
    [Header("Start")]
    [SerializeField] private Collider2D startTrigger;

    [Header("Spawners")]
    [SerializeField] private SpawnerFrutas appleSpawner;
    [SerializeField] private Spawn_viento windSpawner;

    [Header("Anim")]
    [SerializeField] private Animator anim;

    private bool bossStarted = false;
    private bool isDead = false;
    private bool canCheckStart = false;

    private bool isInHit = false;

    private Coroutine bossRoutine;

    private void Start()
    {
        StartCoroutine(EnableStartCheck());
    }

    IEnumerator EnableStartCheck()
    {
        yield return null;
        canCheckStart = true;
    }

    private void Update()
    {
        if (isDead || !canCheckStart) return;

        if (!bossStarted)
            CheckStart();
    }

    void CheckStart()
    {
        Collider2D hit = Physics2D.OverlapBox(
            startTrigger.bounds.center,
            startTrigger.bounds.size,
            0f
        );

        if (hit != null && hit.CompareTag("Player"))
        {
            StartBoss();
        }
    }

    public void StartBoss()
    {
        if (bossStarted) return;

        bossStarted = true;

        anim.Rebind();
        anim.Update(0f);

        anim.Play("Idle", 0, 0f);

        bossRoutine = StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (!isDead)
        {
            yield return StartCoroutine(ApplePhase());
            yield return StartCoroutine(WindPhase());
        }
    }

    IEnumerator ApplePhase()
    {
        float t = 0f;

        while (t < 5f && !isDead)
        {
            appleSpawner.Spawn();

            yield return new WaitForSeconds(1f);
            t += 1f;
        }
    }

    IEnumerator WindPhase()
    {
        float t = 0f;

        while (t < 5f && !isDead)
        {
            anim.ResetTrigger("Hit");
            anim.SetTrigger("Wind");

            windSpawner.SpawnWind();

            yield return new WaitForSeconds(0.7f);

            t += 0.7f;
        }
    }

    // ---------------- DAÑO ----------------

    public override void Damaged(int dmg)
    {
        if (isDead) return;

        base.Damaged(dmg);

        anim.ResetTrigger("Wind");
        anim.SetTrigger("Hit");

        if (health <= 0)
        {
            isDead = true;

            if (bossRoutine != null)
                StopCoroutine(bossRoutine);

            OnDeath();
        }
    }

    IEnumerator HitLock()
    {
        yield return new WaitForSeconds(0.4f);
        isInHit = false;
    }

    // ---------------- MUERTE ----------------

    protected override void OnDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        anim.SetBool("IsDead", true);

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);

        SceneManager.LoadScene("Map-Victory");
    }

    public override bool CanBeAbsorbed()
    {
        return false;
    }
}