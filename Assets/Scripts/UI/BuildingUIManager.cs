using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    public static BuildingUIManager Instance;

    public GameObject[] allPanels;
    public Building CurrentBuilding { get; private set; }

    void Awake()
    {
        Instance = this;
        HideAllPanels();
    }

    public void ShowPanel(GameObject panel, Building building)
    {
        HideAllPanels();
        if (panel != null)
            panel.SetActive(true);

        CurrentBuilding = building;
    }

    public void HideAllPanels()
    {
        foreach (var p in allPanels)
        {
            if (p != null) p.SetActive(false);
        }

        CurrentBuilding = null;
    }
}
