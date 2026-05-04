using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossArbol: comportamiento básico para el jefe árbol.
// - Detecta al jugador (Kirby) en un rango
// - Ejecuta patrones de ataque (proyectiles, slam, spawnear minions)
// - Puede recibir daño y morir
[RequireComponent(typeof(Animator))]
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
    [SerializeField] GameObject projectileVFX;
    [SerializeField] GameObject slamVFX; // Added missing slamVFX field

    [Header("Phases")]
    [SerializeField] int enragedThresholdPercent = 30; // < percent
    [SerializeField] float enragedAttackMultiplier = 0.6f;

    int currentHealth;
    bool isActive = false;
    bool isEnraged = false;

    Transform player;

    Coroutine attackRoutine;
    Animator animator;

    void Awake()
    {
        currentHealth = maxHealth;

        // If animator not assigned in inspector, try to find one on this object or children
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("BossArbol: No Animator found. Please assign an Animator in the inspector.");
        }
    }

    void Start()
    {
        // Intentar obtener el singleton Kirby o buscar el objeto en escena
        TryFindPlayer();

        // Si se configura para activarse al inicio, arrancar comportamiento
        if (startActive)
            ActivateBoss();
    }

    void Update()
    {
        // Si no tenemos referencia al jugador, intentar encontrarla (permite instanciación tardía)
        if (player == null)
            TryFindPlayer();

        // Detección por distancia para activar al jefe
        if (!isActive && player != null)
        {
            float d = Vector2.Distance(player.position, transform.position);
            if (d <= detectionRange)
                ActivateBoss();
        }
    }

    void TryFindPlayer()
    {
        if (Kirby.instance != null)
        {
            player = Kirby.instance.transform;
            return;
        }

        var k = FindObjectOfType<Kirby>();
        if (k != null)
            player = k.transform;
    }

    void ActivateBoss()
    {
        if (isActive) return;
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
            int pick = UnityEngine.Random.Range(0, 2); // 0: Projectile, 1: Slam
            switch (pick)
            {
                case 0:
                    yield return StartCoroutine(ProjectileVolley());
                    break;
                case 1:
                    yield return StartCoroutine(RootSlam());
                    break;
            }

            float wait = timeBetweenAttacks * (isEnraged ? enragedAttackMultiplier : 1f);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator ProjectileVolley()
    {
        if (projectilePrefab == null || projectileSpawns == null || projectileSpawns.Length == 0)
        {
            Debug.LogWarning("BossArbol: No projectilePrefab or projectileSpawns assigned.");
            yield break;
        }

        // Animación del boss para ataque de proyectiles
        if (animator != null)
            animator.SetTrigger("ataque-soplo");
        else
            Debug.LogWarning("BossArbol: Animator is null, cannot trigger ataque-soplo.");

        int shots = isEnraged ? 6 : 4;
        for (int i = 0; i < shots; i++)
        {
            foreach (var sp in projectileSpawns)
            {
                if (sp == null) continue;

                // Capturar la posición actual de Kirby al momento del disparo
                Vector2 targetPosition = player != null ? (Vector2)player.position : Vector2.zero;

                var go = Instantiate(projectilePrefab, sp.position, Quaternion.identity);
                Debug.Log("BossArbol: spawned projectile at " + sp.position);

                // asegurar que el prefab sea visible o instanciar VFX
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    if (projectileVFX == null)
                        Debug.LogWarning("Projectile prefab no tiene SpriteRenderer y no se ha asignado projectileVFX. Asigna un sprite o VFX para verlo.");
                    else
                        Instantiate(projectileVFX, sp.position, Quaternion.identity);
                }

                // marcar el proyectil para facilitar debugging y colisiones
                go.tag = "EnemyAttack";
                go.name = "BossProjectile";
                // asegurar que tiene AttackData para aplicar daño
                var ad = go.GetComponent<AttackData>();
                if (ad == null) ad = go.AddComponent<AttackData>();
                ad.isEnemy = true;
                ad.damage = isEnraged ? 8 : 5;

                // Dirigir el proyectil hacia la posición capturada de Kirby
                Vector2 dir = (targetPosition - (Vector2)sp.position).normalized;
                var rb = go.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = dir * projectileSpeed;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator RootSlam()
    {
        // reproducir animación de golpe
        if (animator != null)
            animator.SetTrigger("SlamAttack");
        else
            Debug.LogWarning("BossArbol: Animator is null, cannot trigger SlamAttack.");

        yield return new WaitForSeconds(0.7f);

        // Instanciar efecto visual del slam
        if (slamVFX != null)
            Instantiate(slamVFX, transform.position, Quaternion.identity);

        // Si tenemos referencia al jugador, usar distancia para aplicar daño (más robusto que layer masks)
        float slamRadius = 3.5f;
        if (player != null)
        {
            float dist = Vector2.Distance(player.position, transform.position);
            if (dist <= slamRadius)
            {
                var k = player.GetComponent<Kirby>();
                if (k != null)
                    k.TakeDamage(2 + (isEnraged ? 2 : 0));
            }
        }
        yield return null;
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

        // reproducir animación de hit
        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("BossArbol muerto");
        // reproducir animación, efectos, soltar loot, etc.
        StopAllCoroutines();
        if (animator != null)
            animator.SetTrigger("Die");
        // Destruir el gameObject al morir (esperar para animación)
        Destroy(gameObject, 0.6f);
    }

    // Método público para aplicar daño desde otros scripts
    public void ApplyDamage(int amount)
    {
        TakeDamage(amount);
    }

    // Si el boss tiene un collider marcado como trigger, permitimos recibir daño de ataques del jugador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Preferir AttackData para determinar daño y origen
        var ad = other.GetComponent<AttackData>();
        if (ad != null)
        {
            // Solo aceptar ataques que no son de enemigos
            if (!ad.isEnemy)
            {
                ApplyDamage(ad.damage);
                Debug.Log("BossArbol: recibido daño de ataque con AttackData=" + ad.damage);
            }
            return;
        }

        // El objeto que daña debe llevar la etiqueta "PlayerAttack" (configurar en prefabs de ataques)
        if (other.CompareTag("PlayerAttack"))
        {
            int damage = 10; // valor por defecto
            ApplyDamage(damage);
            Debug.Log("BossArbol: recibido daño por PlayerAttack tag");
        }
    }

    // Métodos de prueba desde inspector para debuggear animaciones/ataques
    [ContextMenu("Debug Spawn Projectile")]
    void DebugSpawnProjectile()
    {
        if (projectileSpawns != null && projectileSpawns.Length > 0 && projectilePrefab != null)
        {
            var sp = projectileSpawns[0];
            var go = Instantiate(projectilePrefab, sp.position, Quaternion.identity);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null && player != null)
                rb.velocity = (player.position - sp.position).normalized * projectileSpeed;
            Debug.Log("BossArbol Debug: spawn projectile");
        }
        else
        {
            Debug.LogWarning("DebugSpawnProjectile: no hay projectilePrefab o projectileSpawns asignados.");
        }
    }

    [ContextMenu("Debug Slam")]
    void DebugSlam()
    {
        StartCoroutine(RootSlam());
        Debug.Log("BossArbol Debug: slam triggered");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
