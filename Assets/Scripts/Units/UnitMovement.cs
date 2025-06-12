using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector3? targetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (targetPos.HasValue)
        {
            Vector2 direction = ((Vector2)targetPos.Value - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, targetPos.Value);

            if (distance > 0.1f)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                targetPos = null; // Chegou ao destino
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        targetPos = destination;
    }
}
