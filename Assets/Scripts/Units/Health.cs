using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public UnitData unitData;
    public Image healthFill;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; } = false;
    public System.Action OnDeath;

    [Header("Feedback Visual e Sonoro")]
    public AudioClip damageClip;
    public AudioClip deathClip;
    public AudioClip attackClip;

    private AudioSource audioSource;
    private SpriteRenderer sprite;
    private Color originalColor;

    void Start()
    {
        CurrentHealth = unitData.maxHealth;

        audioSource = GetComponent<AudioSource>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null) originalColor = sprite.color;

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
        if (IsDead) return;

        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} recebeu {amount} de dano. Vida atual: {CurrentHealth}");

        UpdateHealthBar();
        PlayDamageFeedback();

        if (attacker != null)
        {
            attacker.GetComponent<UnitCombat>()?.OnAttackedBy(this);
            GetComponent<UnitCombat>()?.OnAttackedBy(attacker);
        }

        if (CurrentHealth <= 0)
            Die();
    }

    private void PlayDamageFeedback()
    {
        if (sprite != null)
            StartCoroutine(FlashRed());

        if (damageClip != null && audioSource != null)
            audioSource.PlayOneShot(damageClip);
    }

    IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
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
        if (IsDead) return;

        IsDead = true;
        Debug.Log($"{gameObject.name} morreu.");

        if (deathClip != null && audioSource != null)
            audioSource.PlayOneShot(deathClip);

        GetComponent<UnitMovement>()?.Stop();

        var anim = GetComponent<UnitAnimationController>();
        if (anim != null)
        {
            anim.CancelInvoke();
            anim.PlayDeath();
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp != this) comp.enabled = false;
        }

        OnDeath?.Invoke();

    }
}
