using UnityEngine;
using System.Collections;
using System.Linq;

public class UnitCombat : MonoBehaviour
{
    public UnitData unitData;
    public float retargetRange = 5f;

    private Health target;
    private float attackCooldown;
    private bool isAttacking;

    private void Update()
    {
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist > unitData.attackRange)
                return;

            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = 1f / unitData.attackRate;
            }
        }
    }

    public void SetTarget(Health newTarget)
    {
        target = newTarget;
        isAttacking = true;
    }

    public void StopAttack()
    {
        target = null;
        isAttacking = false;
    }

    void Attack()
    {
        if (target == null) return;

        Debug.Log($"{gameObject.name} está a atacar {target.gameObject.name} com {unitData.attackDamage} de dano");
        target.TakeDamage(unitData.attackDamage);

        if (target.CurrentHealth <= 0)
        {
            Debug.Log($"{target.gameObject.name} morreu.");
            FindNewTargetNearby();
        }
    }


    void FindNewTargetNearby()
    {
        Health[] potentialTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);
        var enemiesInRange = potentialTargets
            .Where(h => h != null && h != this.GetComponent<Health>())
            .Where(h => Vector2.Distance(transform.position, h.transform.position) <= retargetRange)
            .ToArray();

        if (enemiesInRange.Length > 0)
        {
            SetTarget(enemiesInRange[0]);
        }
        else
        {
            StopAttack();
        }
    }

    public void OnAttackedBy(Health attacker)
    {
        if (!isAttacking)
        {
            SetTarget(attacker);
        }
    }
}