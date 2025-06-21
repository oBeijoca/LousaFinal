using System.Linq;
using System.Collections.Generic;
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

            // === MOVIMENTO NORMAL COM AJUSTE ===
            Debug.Log("Nenhum recurso detetado. A mover unidades para " + worldPosition);
            List<SelectableUnit> selectedUnits = SelectionManager.Instance.GetSelectedUnits();

            int count = selectedUnits.Count;

            if (count == 1)
            {
                var unit = selectedUnits[0];
                unit.GetComponent<VillagerGathering>()?.StopGathering();
                unit.GetComponent<UnitCombat>()?.StopAttack();
                unit.GetComponent<UnitMovement>()?.SetTargetPosition(worldPosition);
            }
            else
            {
                float radius = 0.1f + 0.01f * count; // raio mínimo
                float angleStep = 360f / count;
                float angle = 0f;

                foreach (var unit in selectedUnits)
                {
                    float rad = angle * Mathf.Deg2Rad;
                    Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                    Vector2 finalTarget = worldPosition + offset;

                    unit.GetComponent<VillagerGathering>()?.StopGathering();
                    unit.GetComponent<UnitCombat>()?.StopAttack();
                    unit.GetComponent<UnitMovement>()?.SetTargetPosition(finalTarget);

                    angle += angleStep;
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
