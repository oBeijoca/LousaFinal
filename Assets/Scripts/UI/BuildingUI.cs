using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    public GameObject panel;

    public void SetActive(bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}
