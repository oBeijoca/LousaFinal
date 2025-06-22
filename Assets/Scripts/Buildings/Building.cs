using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName = "Edifício";
    public GameObject actionPanel; // painel UI associado a este edifício

    public virtual void OnSelect()
    {
        Debug.Log("Selecionado: " + buildingName);
        if (actionPanel != null)
        {
            BuildingUIManager.Instance.ShowPanel(actionPanel, this);
        }
    }
}
