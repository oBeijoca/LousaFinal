using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public Texture2D hoverCursor;
    public Texture2D buildCursor;

    public Vector2 hotspot = Vector2.zero;

    public LayerMask enemyLayer;
    public LayerMask interactableLayer;

    public bool buildMode = false;

    private Texture2D currentCursor;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        // Prioridade 0: UI hover
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            SetCursor(hoverCursor);
            return;
        }

        // Prioridade 1: modo construção
        if (buildMode)
        {
            SetCursor(buildCursor);
            return;
        }

        // Prioridade 2: inimigos (espada)
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D enemyHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, enemyLayer);
        if (enemyHit.collider != null)
        {
            SetCursor(attackCursor);
            return;
        }

        // Prioridade 3: interagível (hover)
        RaycastHit2D hoverHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactableLayer);
        if (hoverHit.collider != null)
        {
            SetCursor(hoverCursor);
            return;
        }

        // Prioridade 4: vazio
        SetCursor(defaultCursor);
    }

    void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture != currentCursor)
        {
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
            currentCursor = cursorTexture;
        }
    }

    public void SetBuildMode(bool state)
    {
        buildMode = state;
    }
}
