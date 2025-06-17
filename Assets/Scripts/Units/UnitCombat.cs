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
        if (distance > unitData.attackRange) return;

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
        if (newTarget == selfHealth) return;
        target = newTarget;
        isAttacking = true;
        Debug.Log($"{gameObject.name} definiu novo alvo: {newTarget.gameObject.name}");
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

        Debug.Log($"{gameObject.name} está a atacar {target.gameObject.name} com {unitData.attackDamage} de dano");
        target.TakeDamage(unitData.attackDamage, selfHealth);

        if (target.CurrentHealth <= 0)
        {
            Debug.Log($"{target.gameObject.name} morreu.");
            FindNewTargetNearby();
        }
    }

    void FindNewTargetNearby()
    {
        Health[] all = FindObjectsByType<Health>(FindObjectsSortMode.None);

        var enemiesInRange = all
            .Where(h => h != null && h != selfHealth)
            .Where(h => h.GetComponent<UnitCombat>() != null)
            .Where(h => Vector2.Distance(transform.position, h.transform.position) <= retargetRange)
            .ToArray();

        if (enemiesInRange.Length > 0)
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
        if (!isAttacking && attacker != null && attacker != selfHealth)
        {
            Debug.Log($"{gameObject.name} foi atacado por {attacker.gameObject.name} e vai retaliar.");
            SetTarget(attacker);
        }
    }
}
