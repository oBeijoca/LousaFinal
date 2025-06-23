using System.Collections.Generic;
using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    public static BuildingUIManager Instance;

    [Header("Parent onde os painéis são instanciados (ex: UI/BottomPanel)")]
    [SerializeField] private Transform panelParent;

    [System.Serializable]
    public class BuildingPanelMapping
    {
        public BuildingType buildingType;
        public GameObject panel;
    }

    [SerializeField] private List<BuildingPanelMapping> panelMappings;

    private Dictionary<BuildingType, GameObject> panelMap = new Dictionary<BuildingType, GameObject>();
    private GameObject currentPanelInstance;
    public Building CurrentBuilding { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var mapping in panelMappings)
        {
            if (mapping.panel != null)
            {
                panelMap[mapping.buildingType] = mapping.panel;
            }
        }
    }

    public void ShowPanel(Building building)
    {
        HideCurrentPanel();

        if (building == null || building.buildingData == null)
        {
            Debug.LogWarning("[BuildingUIManager] Edifício ou dados do edifício nulos!");
            return;
        }

        if (panelMap.TryGetValue(building.buildingData.buildingType, out GameObject panel))
        {
            panel.SetActive(true);
            currentPanelInstance = panel;
            Debug.Log($"[BuildingUIManager] Painel {panel.name} ativado.");
        }
        else
        {
            Debug.LogWarning($"[BuildingUIManager] Nenhum painel mapeado para tipo: {building.buildingData.buildingType}");
        }

        CurrentBuilding = building;
    }

    public void HideCurrentPanel()
    {
        if (currentPanelInstance != null)
        {
            currentPanelInstance.SetActive(false);
            currentPanelInstance = null;
        }

        CurrentBuilding = null;
    }
}
