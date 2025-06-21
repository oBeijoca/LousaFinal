using UnityEngine;
using System.Linq;

[RequireComponent(typeof(UnitMovement), typeof(Health))]
public class UnitCombat : MonoBehaviour
{
    public UnitData unitData;
    public float retargetRange = 5f;

    private Health target;
    private float attackCooldown;
    private bool isAttacking;
    private UnitMovement movement;
    private Health selfHealth;

    void Start()
    {
        movement = GetComponent<UnitMovement>();
        selfHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (target == null || !isAttacking) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > unitData.attackRange)
        {
            movement.SetTargetPosition(target.transform.position);
            return;
        }

        if (!movement.ReachedDestination())
        {
            Debug.Log($"{gameObject.name}: Ainda a mover-se. Não ataca.");
            return;
        }

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / unitData.attackRate;
        }
    }

    public void SetTarget(Health newTarget)
    {
        if (newTarget == selfHealth || newTarget == null || newTarget.CurrentHealth <= 0) return;

        target = newTarget;
        isAttacking = true;

        Debug.Log($"{gameObject.name} definiu novo alvo: {newTarget.gameObject.name}");
        movement.SetTargetPosition(newTarget.transform.position);
    }

    public void StopAttack()
    {
        Debug.Log($"{gameObject.name} parou de atacar.");
        target = null;
        isAttacking = false;
    }

    void Attack()
    {
        if (target == null || target == selfHealth) return;

        // ➕ Toca a animação de ataque
        GetComponent<UnitAnimationController>()?.PlayAttack();

        Debug.Log($"{gameObject.name} está a atacar {target.gameObject.name} com {unitData.attackDamage} de dano");
        target.TakeDamage(unitData.attackDamage, selfHealth);

        if (target.CurrentHealth <= 0)
        {
            Debug.Log($"{target.gameObject.name} morreu.");
            Invoke(nameof(FindNewTargetNearby), 0.1f);
        }
    }

    void FindNewTargetNearby()
    {
        Health[] allHealths = FindObjectsByType<Health>(FindObjectsSortMode.None);

        var enemiesInRange = allHealths
            .Where(h => h != null && h != selfHealth && h.CurrentHealth > 0)
            .Where(h => h.GetComponent<UnitCombat>() != null)
            .Where(h => Vector2.Distance(transform.position, h.transform.position) <= retargetRange)
            .Where(h => !h.CompareTag(gameObject.tag))
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .ToList();

        if (enemiesInRange.Count > 0)
        {
            Debug.Log($"{gameObject.name} encontrou novo inimigo: {enemiesInRange[0].gameObject.name}");
            SetTarget(enemiesInRange[0]);
        }
        else
        {
            Debug.Log($"{gameObject.name} não encontrou mais inimigos por perto.");
            StopAttack();
        }
    }

    public void OnAttackedBy(Health attacker)
    {
        if (!isAttacking && attacker != null && attacker != selfHealth && !attacker.CompareTag(gameObject.tag))
        {
            Debug.Log($"{gameObject.name} foi atacado por {attacker.gameObject.name} e vai retaliar.");
            SetTarget(attacker);
        }
    }
}
