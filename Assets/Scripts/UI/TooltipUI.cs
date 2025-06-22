using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI tooltipText;

    private bool isVisible = false;

    void Awake()
    {
        Hide();
    }

    public void Show(string text)
    {
        tooltipText.text = text;

        if (!isVisible)
        {
            panel.SetActive(true);
            isVisible = true;
        }
    }

    public void Hide()
    {
        if (isVisible)
        {
            panel.SetActive(false);
            isVisible = false;
        }
    }
}
