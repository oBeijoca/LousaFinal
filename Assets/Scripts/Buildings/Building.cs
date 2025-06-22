using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName = "Edif�cio";
    public GameObject actionPanel; // painel UI associado a este edif�cio

    public virtual void OnSelect()
    {
        Debug.Log("Selecionado: " + buildingName);
        if (actionPanel != null)
        {
            BuildingUIManager.Instance.ShowPanel(actionPanel, this);
        }
    }
}
