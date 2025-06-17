using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public UnitData unitData;
    public Image healthFill; // opcional – se não for atribuído, tenta encontrar automaticamente

    public int CurrentHealth { get; private set; }
    public System.Action OnDeath;

    void Start()
    {
        CurrentHealth = unitData.maxHealth;

        // Se não foi atribuído manualmente, tenta encontrar automaticamente
        if (healthFill == null)
        {
            Transform auto = transform.Find("Health/HealthBar/BarFill");
            if (auto != null)
            {
                healthFill = auto.GetComponent<Image>();
            }
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} recebeu {amount} de dano. Vida atual: {CurrentHealth}");
        UpdateHealthBar();

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
