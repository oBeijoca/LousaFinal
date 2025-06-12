// Scripts/Units/Unit.cs
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isSelected;
    private SpriteRenderer sr;
    public GameObject selectionCircle;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionCircle != null)
            selectionCircle.SetActive(selected);
    }
}
