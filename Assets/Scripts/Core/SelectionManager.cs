// Scripts/Core/SelectionManager.cs
using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public LayerMask selectableLayer;
    private List<Unit> selectedUnits = new List<Unit>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos, selectableLayer);

            DeselectAll();

            if (hit != null)
            {
                Unit unit = hit.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.SetSelected(true);
                    selectedUnits.Add(unit);
                }
            }
        }
    }

    void DeselectAll()
    {
        foreach (Unit u in selectedUnits)
            u.SetSelected(false);
        selectedUnits.Clear();
    }
}
