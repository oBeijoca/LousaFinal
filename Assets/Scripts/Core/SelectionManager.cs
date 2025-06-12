using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public LayerMask selectableLayer;
    private List<Unit> selectedUnits = new List<Unit>();
    public GameObject clickMarkerPrefab;
    private GameObject currentClickMarker;

    void Update()
    {
        // Seleção com botão esquerdo
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos, selectableLayer);

            bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (!shiftHeld)
                DeselectAll();

            if (hit != null)
            {
                Unit unit = hit.GetComponent<Unit>();
                if (unit != null)
                {
                    if (selectedUnits.Contains(unit))
                    {
                        // Com shift: se já estiver selecionada, desmarcar
                        if (shiftHeld)
                        {
                            unit.SetSelected(false);
                            selectedUnits.Remove(unit);
                        }
                    }
                    else
                    {
                        unit.SetSelected(true);
                        selectedUnits.Add(unit);
                    }
                }
            }
        }


        // Movimento com botão direito
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Unit unit in selectedUnits)
            {
                UnitMovement move = unit.GetComponent<UnitMovement>();
                if (move != null)
                    move.MoveTo(destination);
            }

            // Criar marcador visual do clique
            if (clickMarkerPrefab != null)
            {
                if (currentClickMarker != null)
                    Destroy(currentClickMarker);

                currentClickMarker = Instantiate(clickMarkerPrefab, destination, Quaternion.identity);
            }
        }
    }

    void DeselectAll()
    {
        foreach (Unit u in selectedUnits)
            u.SetSelected(false);
        selectedUnits.Clear();
    }

    public void AddToSelection(Unit unit)
    {
        if (!selectedUnits.Contains(unit))
            selectedUnits.Add(unit);
    }

    public void DeselectAllPublic()
    {
        DeselectAll();
    }

}
