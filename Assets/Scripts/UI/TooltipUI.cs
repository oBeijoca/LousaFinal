using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject panel;
    public TextMeshProUGUI tooltipText;
    private RectTransform canvasRect;

    void Awake()
    {
        Instance = this;
        canvasRect = transform.parent.GetComponent<RectTransform>();
        Hide();
    }

    void Update()
    {
        if (panel.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null,
                out pos);

            Vector2 offset = new Vector2(10f, -10f);
            panel.transform.localPosition = pos + offset;
        }
    }

    public void Show(string text)
    {
        tooltipText.raycastTarget = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        tooltipText.text = text;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
