using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dragSpeed = 2f;
    public float edgeSize = 15f;
    public float edgeMoveSpeed = 1.5f;
    public float zoomSpeed = 2f;
    public float minZoom = 0.5f;
    public float maxZoom = 1.5f;

    public float minX = -1f, maxX = 21f, minY = -1f, maxY = 21f;

    [Header("Tilemap Bounds")]
    public Tilemap referenceTilemap;

    private bool edgeMovementEnabled = true;
    private Vector3 dragOrigin;
    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    [Header("Selection Reference")]
    public SelectionManager selectionManager;

    void Start()
    {
        cam = Camera.main;

        if (referenceTilemap != null)
        {
            Bounds tileBounds = referenceTilemap.localBounds;

            minX = tileBounds.min.x;
            maxX = tileBounds.max.x;
            minY = tileBounds.min.y;
            maxY = tileBounds.max.y;

            Debug.Log($"[CameraController] Limites carregados do tilemap: ({minX}, {maxX}), ({minY}, {maxY})");
        }
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.LeftArrow)) pos.x -= moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) pos.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) pos.y += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) pos.y -= moveSpeed * Time.deltaTime;

        if (Input.GetMouseButtonDown(2))
            dragOrigin = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            Vector3 diff = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-diff.x * dragSpeed, -diff.y * dragSpeed, 0);
            pos += move;
            dragOrigin = Input.mousePosition;
        }

        if (edgeMovementEnabled)
        {
            Vector2 mouse = Input.mousePosition;
            if (mouse.x <= edgeSize) pos.x -= edgeMoveSpeed * Time.deltaTime;
            if (mouse.x >= Screen.width - edgeSize) pos.x += edgeMoveSpeed * Time.deltaTime;
            if (mouse.y <= edgeSize) pos.y -= edgeMoveSpeed * Time.deltaTime;
            if (mouse.y >= Screen.height - edgeSize) pos.y += edgeMoveSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3? center = GetSelectionCenter();
            if (center.HasValue)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothMove(center.Value));
            }
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            edgeMovementEnabled = !edgeMovementEnabled;
            Debug.Log("Movimento pelas bordas: " + (edgeMovementEnabled ? "ligado" : "desligado"));
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    Vector3? GetSelectionCenter()
    {
        if (selectionManager == null) return null;

        List<Transform> selected = selectionManager.GetSelectedTransforms();
        if (selected.Count == 0) return null;

        Vector3 total = Vector3.zero;
        foreach (var t in selected)
            total += t.position;

        return total / selected.Count;
    }

    System.Collections.IEnumerator SmoothMove(Vector3 target)
    {
        target.z = transform.position.z;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.2f);
            yield return null;
        }
    }
}
