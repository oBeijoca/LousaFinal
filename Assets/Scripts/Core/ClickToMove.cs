using System.Linq;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private GameObject clickMarkerPrefab;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Debug.Log("ClickToMove iniciado. Câmara encontrada? " + (mainCamera != null));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            Debug.Log("Botão direito clicado na posição do rato: " + mousePosition + " | Posição no mundo: " + worldPosition);

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
            Debug.Log("Colisores encontrados: " + hits.Length);

            foreach (var hit in hits)
            {
                Debug.Log("Verificando colisão com: " + hit.name);

                // === RECOLHA DE RECURSOS ===
                ResourceNode resource = hit.GetComponent<ResourceNode>();
                if (resource != null)
                {
                    Debug.Log("Clicou num recurso: " + resource.name);
                    foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
                    {
                        VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                        if (gatherer != null)
                        {
                            GameObject depositBuilding = GameObject.FindWithTag("Deposit");
                            Transform depositPoint = depositBuilding?.transform.Find("DepositPoint") ?? depositBuilding?.transform;

                            gatherer.StartGathering(resource, depositPoint);
                        }
                    }
                    ShowMarker(worldPosition);
                    return;
                }

                // === COMBATE ===
                Health targetHealth = hit.GetComponent<Health>();
                if (targetHealth != null)
                {
                    Debug.Log("Clicou num inimigo: " + hit.name);
                    foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
                    {
                        UnitCombat combat = unit.GetComponent<UnitCombat>();
                        if (combat != null)
                        {
                            combat.SetTarget(targetHealth);
                        }

                        UnitMovement movement = unit.GetComponent<UnitMovement>();
                        if (movement != null)
                        {
                            movement.SetTargetPosition(targetHealth.transform.position);
                        }

                        VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                        if (gatherer != null) gatherer.StopGathering();
                    }

                    ShowMarker(worldPosition);
                    return;
                }
            }

            // === MOVIMENTO NORMAL ===
            Debug.Log("Nenhum recurso detetado. A mover unidade para " + worldPosition);
            foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
            {
                VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                if (gatherer != null) gatherer.StopGathering();

                UnitCombat combat = unit.GetComponent<UnitCombat>();
                if (combat != null) combat.StopAttack();

                UnitMovement movement = unit.GetComponent<UnitMovement>();
                if (movement != null)
                {
                    Debug.Log("A mover unidade: " + unit.name + " para " + worldPosition);
                    movement.SetTargetPosition(worldPosition);
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
