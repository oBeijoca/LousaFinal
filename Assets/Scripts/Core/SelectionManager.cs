using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    private List<SelectableUnit> selectedObjects = new List<SelectableUnit>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // Tenta selecionar unidade
                SelectableUnit unit = hit.collider.GetComponent<SelectableUnit>();
                if (unit != null)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                        Deselect();

                    Select(unit);
                    return;
                }

                // Tenta selecionar edifício
                Building building = hit.collider.GetComponent<Building>();
                if (building != null)
                {
                    Deselect(); // só uma construção de cada vez
                    building.OnSelect();
                    return;
                }
            }
            else
            {
                // Só deseleciona se o clique não for em UI
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    Deselect();
            }

        }
    }

    public void Select(SelectableUnit unit)
    {
        if (unit != null && !selectedObjects.Contains(unit))
        {
            selectedObjects.Add(unit);
            unit.SetSelected(true);

            var health = unit.GetComponent<Health>();
            if (health != null && health.unitData != null && health.unitData.unitType == UnitData.UnitType.Villager)
            {
                UnitUI.Instance?.Show();
                BuildingUIManager.Instance.HideAllPanels();
            }
            else
            {
                UnitUI.Instance?.Hide();
                BuildingUIManager.Instance.HideAllPanels();
            }
        }
    }

    public void AddToSelection(SelectableUnit unit)
    {
        Select(unit);
    }

    public void Deselect()
    {
        foreach (var obj in selectedObjects)
        {
            if (obj != null)
                obj.SetSelected(false);
        }

        selectedObjects.Clear();

        UnitUI.Instance?.Hide();
        BuildingUIManager.Instance.HideAllPanels();
    }

    public void DeselectAllPublic()
    {
        Deselect();
    }

    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedObjects;
    }

    public List<Transform> GetSelectedTransforms()
    {
        List<Transform> result = new List<Transform>();
        foreach (var obj in selectedObjects)
        {
            if (obj != null)
                result.Add(obj.transform);
        }

        return result;
    }
}
