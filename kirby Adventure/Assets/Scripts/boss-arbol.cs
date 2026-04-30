using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossArbol: comportamiento básico para el jefe árbol.
// - Detecta al jugador (Kirby) en un rango
// - Ejecuta patrones de ataque (proyectiles, slam, spawnear minions)
// - Puede recibir daño y morir
public class BossArbol : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 50;
    [SerializeField] float detectionRange = 12f;
    [SerializeField] bool startActive = false;

    [Header("Attacks")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform[] projectileSpawns;
    [SerializeField] float projectileSpeed = 6f;
    [SerializeField] float timeBetweenAttacks = 3f;
    [SerializeField] GameObject minionPrefab;
    [SerializeField] Transform[] minionSpawnPoints;

    [Header("Phases")]
    [SerializeField] int enragedThresholdPercent = 30; // < percent
    [SerializeField] float enragedAttackMultiplier = 0.6f;

    int currentHealth;
    bool isActive = false;
    bool isEnraged = false;

    Transform player;

    Coroutine attackRoutine;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Intentar obtener el singleton Kirby o buscar el objeto en escena
        if (Kirby.instance != null)
            player = Kirby.instance.transform;
        else
            player = FindObjectOfType<Kirby>()?.transform;

        if (startActive)
            ActivateBoss();
    }

    void Update()
    {
        if (!isActive && player != null)
        {
            float d = Vector2.Distance(player.position, transform.position);
            if (d <= detectionRange)
                ActivateBoss();
        }
    }

    void ActivateBoss()
    {
        isActive = true;
        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        while (currentHealth > 0)
        {
            // comprobar fase de enrage
            CheckEnrage();

            // Elegir ataque aleatorio
            int pick = Random.Range(0, 3);
            switch (pick)
            {
                case 0:
                    yield return StartCoroutine(ProjectileVolley());
                    break;
                case 1:
                    yield return StartCoroutine(RootSlam());
                    break;
                case 2:
                    yield return StartCoroutine(SpawnMinions());
                    break;
            }

            float wait = timeBetweenAttacks * (isEnraged ? enragedAttackMultiplier : 1f);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator ProjectileVolley()
    {
        if (projectilePrefab == null || projectileSpawns == null || projectileSpawns.Length == 0)
            yield break;

        int shots = isEnraged ? 6 : 4;
        for (int i = 0; i < shots; i++)
        {
            foreach (var sp in projectileSpawns)
            {
                if (sp == null) continue;
                var go = Instantiate(projectilePrefab, sp.position, Quaternion.identity);
                // intentar dirigir al jugador si existe
                Vector2 dir = (player != null) ? (player.position - sp.position).normalized : (Vector2.left);
                var rb = go.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = dir * projectileSpeed;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator RootSlam()
    {
        // animación / telegraph could be added
        // Simple implementation: create an area damage by enabling a collider or instantiating an effect
        // For now wait a bit to simulate windup
        yield return new WaitForSeconds(0.7f);
        // TODO: apply damage to player if inside range
        // We can use a Physics2D.OverlapCircle to detect player
        float slamRadius = 3.5f;
        Collider2D hit = Physics2D.OverlapCircle(transform.position, slamRadius, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            var k = hit.GetComponent<Kirby>();
            if (k != null)
            {
                k.TakeDamage(2 + (isEnraged ? 2 : 0));
            }
        }
        yield return null;
    }

    IEnumerator SpawnMinions()
    {
        if (minionPrefab == null || minionSpawnPoints == null || minionSpawnPoints.Length == 0)
            yield break;

        int toSpawn = isEnraged ? 3 : 2;
        for (int i = 0; i < toSpawn; i++)
        {
            foreach (var sp in minionSpawnPoints)
            {
                if (sp == null) continue;
                Instantiate(minionPrefab, sp.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void CheckEnrage()
    {
        if (isEnraged) return;
        float percent = (100f * currentHealth) / Mathf.Max(1, maxHealth);
        if (percent <= enragedThresholdPercent)
            isEnraged = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        // pequeño feedback
        Debug.Log("BossArbol: daño recibido " + amount + " - vida restante: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("BossArbol muerto");
        // reproducir animación, efectos, soltar loot, etc.
        StopAllCoroutines();
        // Destruir el gameObject al morir
        Destroy(gameObject);
    }

    // Método público para aplicar daño desde otros scripts
    public void ApplyDamage(int amount)
    {
        TakeDamage(amount);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
