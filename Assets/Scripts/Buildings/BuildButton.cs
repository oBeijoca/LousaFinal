using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public GameObject placerPrefab; // Prefab que cont�m o BuildingPlacer configurado com o tipo de constru��o

    public void OnClick()
    {
        if (placerPrefab == null)
        {
            Debug.LogError("Placer prefab n�o atribu�do no bot�o de constru��o.");
            return;
        }

        if (FindAnyObjectByType<BuildingPlacer>() != null)
        {
            Debug.Log("J� h� um BuildingPlacer ativo.");
            return;
        }

        Instantiate(placerPrefab);
        CursorManager.Instance.SetBuildMode(true); // Altera o cursor
    }
}
