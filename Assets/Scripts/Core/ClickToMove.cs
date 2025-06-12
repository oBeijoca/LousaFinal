using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public GameObject clickMarkerPrefab;
    private GameObject currentMarker;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // botão direito
        {
            // Converte posição do rato para coordenadas do mundo
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f; // Garante que está no plano 2D

            Debug.Log("Clique em: " + worldPosition);

            var selectedUnits = SelectionManager.Instance.GetSelectedUnits();
            Debug.Log("Unidades selecionadas: " + selectedUnits.Count);

            foreach (var unit in selectedUnits)
            {
                UnitMovement movement = unit.GetComponent<UnitMovement>();
                if (movement != null)
                {
                    Debug.Log("Movendo unidade: " + unit.name);
                    movement.SetDestination(worldPosition);
                }
                else
                {
                    Debug.LogWarning("UnitMovement não encontrado em: " + unit.name);
                }
            }

            if (clickMarkerPrefab != null)
            {
                if (currentMarker != null)
                    Destroy(currentMarker);

                currentMarker = Instantiate(clickMarkerPrefab, worldPosition, Quaternion.identity);
            }
        }
    }
}
