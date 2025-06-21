using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

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
            PlaceBuilding(mouseWorldPos);
        }
        // Cancelar com botão direito
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void PlaceBuilding(Vector2 position)
    {
        Destroy(ghostObject);
        GameObject final = Instantiate(buildingPrefab, position, Quaternion.identity);
        // Aqui podíamos instanciar um ConstructionSite temporário no lugar do final
        Destroy(this.gameObject); // remove o placer
    }

    void CancelPlacement()
    {
        Destroy(ghostObject);
        Destroy(this.gameObject);
    }
}
