using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData buildingData; // Dados do edif�cio

    public virtual void OnSelect()
    {
        if (buildingData != null)
        {
            Debug.Log("Selecionado: " + buildingData.buildingName);
            BuildingUIManager.Instance.ShowPanel(this);
        }
        else
        {
            Debug.LogWarning("[Building] Nenhum BuildingData atribu�do ao edif�cio " + gameObject.name);
        }
    }

}
