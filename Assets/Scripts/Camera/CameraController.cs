using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float dragSpeed = 1f;
    public float edgeSize = 20f;
    public float edgeMoveSpeed = 1.5f;
    public float zoomSpeed = 2f;
    public float minZoom = 0.5f;
    public float maxZoom = 1.5f;

    [Header("Camera Movement Bounds")]
    public float minX = -10f, maxX = 10f, minY = -10f, maxY = 10f;

    private bool edgeMovementEnabled = true;
    private Vector3 dragOrigin;
    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    [Header("Selection Reference")]
    public SelectionManager selectionManager;

    void Start()
    {
        cam = Camera.main;

        float orthoSize = cam.orthographicSize;
        float camHalfWidth = orthoSize * cam.aspect;

        // Ajustar os limites com margem visual
        minX += camHalfWidth;
        maxX -= camHalfWidth;
        minY += orthoSize;
        maxY -= orthoSize;


        // Força a posição inicial da câmara dentro dos limites
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
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

        // Limita dentro dos limites definidos
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

            // Clamp dentro da coroutine também
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);

            yield return null;
        }
    }
}
