using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName = "Centro da Vila";
    public GameObject unitToSpawn;
    public Transform spawnPoint;

    public virtual void OnSelect()
    {
        Debug.Log("Selecionado: " + buildingName);
        BuildingUI.Instance.Show(this);
    }

    public virtual void SpawnUnit()
    {
        if (unitToSpawn != null && spawnPoint != null)
        {
            Instantiate(unitToSpawn, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("unitToSpawn ou spawnPoint não atribuídos.");
        }
    }

}
