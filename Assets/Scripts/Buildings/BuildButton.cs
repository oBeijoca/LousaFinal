using UnityEngine;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject placerPrefab; // Prefab que contém o BuildingPlacer configurado com o tipo de construção

    public int woodCost;
    public int stoneCost;
    public int goldCost;
    public int foodCost;

    public string buildingName;
    public TooltipUI tooltipUI; // Tooltip específico deste botão

    public void OnClick()
    {
        if (placerPrefab == null)
        {
            Debug.LogError("Placer prefab não atribuído no botão de construção.");
            return;
        }

        if (FindAnyObjectByType<BuildingPlacer>() != null)
        {
            Debug.Log("Já há um BuildingPlacer ativo.");
            return;
        }

        GameObject placer = Instantiate(placerPrefab);
        var placerScript = placer.GetComponent<BuildingPlacer>();
        if (placerScript != null)
        {
            placerScript.woodCost = woodCost;
            placerScript.stoneCost = stoneCost;
            placerScript.goldCost = goldCost;
            placerScript.foodCost = foodCost;
        }

        CursorManager.Instance.SetBuildMode(true); // Altera o cursor
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI != null)
        {
            string text = $"{buildingName}\nMadeira: {woodCost}\nPedra: {stoneCost}\nOuro: {goldCost}\nComida: {foodCost}";
            tooltipUI.Show(text);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI?.Hide();
    }
}
