using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectionBox : MonoBehaviour
{
    public RectTransform selectionBox;
    private Vector2 startPos;
    private Vector2 endPos;
    private Camera cam;
    public SelectionManager selectionManager;
    private bool isDragging = false;
    private float dragThreshold = 10f; // pixels

    void Start()
    {
        cam = Camera.main;
        selectionBox.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector2.Distance(Input.mousePosition, startPos) > dragThreshold)
            {
                isDragging = true;
                selectionBox.gameObject.SetActive(true);
            }

            if (isDragging)
            {
                endPos = Input.mousePosition;
                Vector2 size = endPos - startPos;

                selectionBox.anchoredPosition = startPos;
                selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
                selectionBox.pivot = new Vector2(size.x >= 0 ? 0 : 1, size.y >= 0 ? 0 : 1);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                SelectUnitsInBox();
                selectionBox.gameObject.SetActive(false);
            }
        }
    }


    void SelectUnitsInBox()
    {
        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);

        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (!shiftHeld)
            selectionManager.DeselectAllPublic();

        foreach (SelectableUnit unit in Object.FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None))
        {
            Vector2 screenPos = cam.WorldToScreenPoint(unit.transform.position);
            if (screenPos.x >= min.x && screenPos.x <= max.x &&
                screenPos.y >= min.y && screenPos.y <= max.y)
            {
                unit.SetSelected(true);
                SelectionManager.Instance.AddToSelection(unit);
            }
        }

    }
}
