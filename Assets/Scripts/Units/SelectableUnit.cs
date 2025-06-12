using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SelectableUnit : MonoBehaviour
{
    public GameObject selectionCircle;
    public bool isSelected { get; private set; }

    private SpriteRenderer sr;

    void Start()
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
