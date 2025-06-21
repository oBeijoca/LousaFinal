using UnityEngine;

public class UnitUI : MonoBehaviour
{
    public static UnitUI Instance;

    public GameObject actionPanel;

    void Awake()
    {
        Instance = this;
        actionPanel.SetActive(false);
    }

    public void Show()
    {
        actionPanel.SetActive(true);
    }

    public void Hide()
    {
        actionPanel.SetActive(false);
    }
}
