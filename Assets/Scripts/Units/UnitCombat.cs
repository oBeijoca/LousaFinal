using UnityEngine;
using System.Linq;

[RequireComponent(typeof(UnitMovement), typeof(Health), typeof(Rigidbody2D))]
public class UnitCombat : MonoBehaviour
{
    public UnitData unitData;
    public float retargetRange = 5f;

    private Health target;
    private float attackCooldown;
    private bool isAttacking;

    private UnitMovement movement;
    private UnitAnimationController animController;
    private Health selfHealth;
    private Rigidbody2D rb;

    private bool loggedIdleReset = false;

    void Start()
    {
        movement = GetComponent<UnitMovement>();
        animController = GetComponent<UnitAnimationController>();
        selfHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isAttacking || target == null || target.IsDead || selfHealth.IsDead)
        {
            StopAttack();
            return;
        }

        Vector2 closestPoint = target.GetComponent<Collider2D>()?.ClosestPoint(transform.position) ?? target.transform.position;
        float distance = Vector2.Distance(transform.position, closestPoint);

        if (distance > unitData.attackRange)
        {
            movement.SetTargetPosition(closestPoint);
            animController?.PlayWalk();
        }
        else
        {
            movement.Stop();
            rb.linearVelocity = Vector2.zero;

            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                AttackTarget();
                attackCooldown = 1f / unitData.attackRate;
            }
        }
    }

    void AttackTarget()
    {
        if (target == null || target.IsDead || selfHealth.IsDead) return;

        target.TakeDamage(unitData.attackDamage, selfHealth);
        Debug.Log($"{gameObject.name} atacou {target.gameObject.name} causando {unitData.attackDamage} dano");

        if (target == null || target.IsDead)
        {
            string killedName = target?.gameObject.name; // guardar antes que se perca
            StopAttack();

            if (!loggedIdleReset)
            {
                Debug.LogWarning($"[DEBUG] {gameObject.name} matou {killedName ?? "(null)"} e vai forçar Idle.");
                loggedIdleReset = true;
            }

            animController?.ResetToIdle();
            Invoke(nameof(FindNewTargetNearby), 0.2f);
            return;
        }


        Vector2 delta = target.transform.position - transform.position;
        animController?.PlayAttack(delta);
    }

    public void SetTarget(Health newTarget)
    {
        if (newTarget == null || newTarget == selfHealth || newTarget.IsDead || selfHealth.IsDead) return;

        target = newTarget;
        isAttacking = true;
        attackCooldown = 0f;
        loggedIdleReset = false;

        Debug.Log($"{gameObject.name} novo alvo: {target.gameObject.name}");
    }

    public void StopAttack()
    {
        if (!isAttacking && target == null) return;

        Debug.Log($"{gameObject.name} parou de atacar.");
        isAttacking = false;
        target = null;

        movement.Stop();
        animController?.ResetToIdle();
    }

    void FindNewTargetNearby()
    {
        if (selfHealth.IsDead) return;

        var allHealth = FindObjectsByType<Health>(FindObjectsSortMode.None);
        var enemies = allHealth
            .Where(h => h != selfHealth && !h.IsDead && h.CurrentHealth > 0)
            .Where(h => !h.CompareTag(gameObject.tag))
            .Where(h => Vector2.Distance(transform.position, h.transform.position) <= retargetRange)
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .ToList();

        if (enemies.Count > 0)
        {
            SetTarget(enemies[0]);
        }
    }

    public void OnAttackedBy(Health attacker)
    {
        if (!isAttacking && attacker != null && !attacker.IsDead && attacker != selfHealth && !attacker.CompareTag(gameObject.tag))
        {
            SetTarget(attacker);
        }
    }
}
