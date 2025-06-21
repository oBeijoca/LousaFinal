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

    void Start()
    {
        movement = GetComponent<UnitMovement>();
        animController = GetComponent<UnitAnimationController>();
        selfHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isAttacking || target == null || target.CurrentHealth <= 0)
        {
            StopAttack();
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > unitData.attackRange * 0.95f)
        {
            movement.SetTargetPosition(GetAttackPosition(target.transform.position));
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

    Vector2 GetAttackPosition(Vector2 targetPos)
    {
        Vector2 direction = (transform.position - (Vector3)targetPos).normalized;
        return targetPos + direction * (unitData.attackRange * 0.9f);
    }

    void AttackTarget()
    {
        if (target == null || target.CurrentHealth <= 0) return;

        target.TakeDamage(unitData.attackDamage, selfHealth);
        Debug.Log($"{gameObject.name} atacou {target.gameObject.name} causando {unitData.attackDamage} dano");

        Vector2 delta = target.transform.position - transform.position;
        animController?.PlayAttack(delta);

        if (target.CurrentHealth <= 0)
        {
            StopAttack();
            Invoke(nameof(FindNewTargetNearby), 0.2f);
        }
    }

    public void SetTarget(Health newTarget)
    {
        if (newTarget == null || newTarget == selfHealth || newTarget.CurrentHealth <= 0) return;

        target = newTarget;
        isAttacking = true;
        attackCooldown = 0f;

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
        var allHealth = FindObjectsByType<Health>(FindObjectsSortMode.None);
        var enemies = allHealth
            .Where(h => h != selfHealth && h.CurrentHealth > 0)
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
        if (!isAttacking && attacker != null && attacker != selfHealth && !attacker.CompareTag(gameObject.tag))
        {
            SetTarget(attacker);
        }
    }
}
