using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData buildingData; // Dados do edifício

    public virtual void OnSelect()
    {
        if (buildingData != null)
        {
            Debug.Log("Selecionado: " + buildingData.buildingName);
            BuildingUIManager.Instance.ShowPanel(this);
        }
        else
        {
            Debug.LogWarning("[Building] Nenhum BuildingData atribuído ao edifício " + gameObject.name);
        }
    }

}
