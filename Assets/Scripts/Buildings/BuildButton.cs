using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public GameObject placerPrefab; // Prefab que contém o BuildingPlacer configurado com o tipo de construção

    public void OnClick()
    {
        if (placerPrefab == null)
        {
            Debug.LogError("Placer prefab não atribuído no botão de construção.");
            return;
        }

        Instantiate(placerPrefab);
        CursorManager.Instance.SetBuildMode(true); // Altera o cursor
    }
}
