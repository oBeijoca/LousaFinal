using UnityEngine;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject placerPrefab; // Prefab que cont�m o BuildingPlacer configurado com o tipo de constru��o

    public int woodCost;
    public int stoneCost;
    public int goldCost;
    public int foodCost;

    public string buildingName;
    public TooltipUI tooltipUI; // Tooltip espec�fico deste bot�o

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
