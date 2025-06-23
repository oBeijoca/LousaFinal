using TMPro;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    public int populationMax = 10;
    public int populationCurrent = 0;

    public TextMeshProUGUI populationText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        populationCurrent = GameObject.FindGameObjectsWithTag("Player").Length;
        UpdateUI();
    }

    public void IncreasePopulationCap(int amount)
    {
        populationMax += amount;
        UpdateUI();
    }

    public bool CanRecruit() => populationCurrent < populationMax;

    private void UpdateUI()
    {
        if (populationText != null)
            populationText.text = $"{populationCurrent}/{populationMax}";
    }
}
