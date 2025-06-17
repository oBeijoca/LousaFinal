using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public UnitData unitData;
    public Image healthFill;
    public int CurrentHealth { get; private set; }
    public System.Action OnDeath;

    void Start()
    {
        CurrentHealth = unitData.maxHealth;

        if (healthFill == null)
        {
            Transform auto = transform.Find("Health/HealthBar/BarFill");
            if (auto != null)
                healthFill = auto.GetComponent<Image>();
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int amount, Health attacker = null)
    {
        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} recebeu {amount} de dano. Vida atual: {CurrentHealth}");

        UpdateHealthBar();

        if (attacker != null)
        {
            attacker.GetComponent<UnitCombat>()?.OnAttackedBy(this);
            GetComponent<UnitCombat>()?.OnAttackedBy(attacker);
        }

        if (CurrentHealth <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            float ratio = (float)CurrentHealth / unitData.maxHealth;
            healthFill.fillAmount = ratio;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} morreu.");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
