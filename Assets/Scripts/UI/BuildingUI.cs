using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    public GameObject actionPanel;
    public Button recruitVillagerButton;

    private Building activeBuilding;

    void Awake()
    {
        Instance = this;
        actionPanel.SetActive(false);

        // Ligar botão ao método local
        recruitVillagerButton.onClick.AddListener(OnRecruitVillagerPressed);
    }

    public void Show(Building building)
    {
        gameObject.SetActive(true);
        actionPanel.SetActive(true);
        activeBuilding = building;
    }

    public void Hide()
    {
        actionPanel.SetActive(false);
        activeBuilding = null;
    }

    void OnRecruitVillagerPressed()
    {
        if (activeBuilding != null)
            activeBuilding.SpawnUnit();
    }
}
