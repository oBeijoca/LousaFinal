using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private GameObject clickMarkerPrefab;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Botão direito
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
            foreach (var hitCollider in hits)
            {
                ResourceNode resource = hitCollider.GetComponent<ResourceNode>();
                if (resource != null)
                {
                    Debug.Log("Clicou num recurso: " + resource.name);
                    foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
                    {
                        VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                        if (gatherer != null)
                        {
                            Transform townCenter = GameObject.FindWithTag("TownCenter")?.transform;
                            if (townCenter != null)
                                gatherer.StartGathering(resource, townCenter);
                        }
                    }

                    ShowMarker(worldPosition);
                    return;
                }
            }

            // Caso não tenha sido um recurso andar
            foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
            {
                UnitMovement movement = unit.GetComponent<UnitMovement>();
                if (movement != null)
                {
                    movement.SetDestination(worldPosition);
                }
            }

            ShowMarker(worldPosition);
        }
    }

    private void ShowMarker(Vector2 position)
    {
        if (clickMarkerPrefab != null)
        {
            GameObject existing = GameObject.FindWithTag("ClickMarker");
            if (existing != null) Destroy(existing);

            GameObject marker = Instantiate(clickMarkerPrefab, position, Quaternion.identity);
            marker.tag = "ClickMarker";
        }
    }
}
