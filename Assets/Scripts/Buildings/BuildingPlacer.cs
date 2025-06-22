using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab; // o edifício final que o jogador vê
    public GameObject constructionSitePrefab; // o que é instanciado primeiro
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    public int woodCost;
    public int stoneCost;
    public int goldCost;
    public int foodCost;

    private GameObject ghostObject;
    private SpriteRenderer ghostRenderer;
    private bool isValidPlacement = false;

    void Start()
    {
        ghostObject = Instantiate(buildingPrefab);
        ghostRenderer = ghostObject.GetComponent<SpriteRenderer>();
        ghostObject.GetComponent<Collider2D>().enabled = false; // evitar interações durante pré-visualização
    }

    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ghostObject.transform.position = mouseWorldPos;

        // Verificar colisões
        Collider2D[] colliders = Physics2D.OverlapBoxAll(mouseWorldPos, ghostRenderer.bounds.size, 0);
        isValidPlacement = colliders.Length == 0;

        // Atualizar cor
        ghostRenderer.color = isValidPlacement ? validColor : invalidColor;

        // Confirmar construção
        if (Input.GetMouseButtonDown(0) && isValidPlacement && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            TryPlaceBuilding(mouseWorldPos);
        }
        // Cancelar com botão direito
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void TryPlaceBuilding(Vector2 position)
    {
        var rm = ResourceManager.Instance;
        bool hasResources = rm.GetResourceAmount(ResourceNode.ResourceType.Wood) >= woodCost &&
                            rm.GetResourceAmount(ResourceNode.ResourceType.Stone) >= stoneCost &&
                            rm.GetResourceAmount(ResourceNode.ResourceType.Gold) >= goldCost &&
                            rm.GetResourceAmount(ResourceNode.ResourceType.Food) >= foodCost;

        if (!hasResources)
        {
            Debug.Log("Recursos insuficientes no momento da colocação.");
            return;
        }

        // Gasta os recursos agora
        rm.SpendResource(ResourceNode.ResourceType.Wood, woodCost);
        rm.SpendResource(ResourceNode.ResourceType.Stone, stoneCost);
        rm.SpendResource(ResourceNode.ResourceType.Gold, goldCost);
        rm.SpendResource(ResourceNode.ResourceType.Food, foodCost);

        PlaceBuilding(position);
    }

    void PlaceBuilding(Vector2 position)
    {
        Destroy(ghostObject);
        CursorManager.Instance.SetBuildMode(false);

        GameObject site = Instantiate(constructionSitePrefab, position, Quaternion.identity);
        var siteScript = site.GetComponent<ConstructionSite>();
        siteScript.finalBuildingPrefab = buildingPrefab;

        // Enviar o aldeão para o local
        var selectedUnits = SelectionManager.Instance.GetSelectedUnits();
        if (selectedUnits.Count > 0)
        {
            var villager = selectedUnits[0];
            var movement = villager.GetComponent<UnitMovement>();
            if (movement != null)
                movement.SetTargetPosition(position);
        }

        Destroy(gameObject); // remove o placer
    }

    void CancelPlacement()
    {
        Destroy(ghostObject);
        CursorManager.Instance.SetBuildMode(false);
        Destroy(gameObject);
    }
}
