using UnityEngine;

[ExecuteAlways]
public class CameraBoundsDebugger : MonoBehaviour
{
    public float minX, maxX, minY, maxY;
    private Camera cam;

    void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Gizmos.color = Color.red;

        // Desenha limites
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(minX, maxY));
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(maxX, maxY));
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(maxX, minY));
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(minX, minY));

        // Desenha campo de visão da câmara
        Gizmos.color = Color.green;
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        Gizmos.DrawWireCube(new Vector3(camPos.x, camPos.y, 0), new Vector3(camWidth, camHeight, 0));
    }
}
