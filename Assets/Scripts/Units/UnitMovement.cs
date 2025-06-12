using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector3? targetPos;
    private float stuckTimer = 0f;
    private float unstuckCooldown = 0f;
    private const float STUCK_DURATION = 1.0f;
    private const float UNSTUCK_COOLDOWN_TIME = 2.0f;

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

        if (unstuckCooldown > 0f)
            unstuckCooldown -= Time.deltaTime;

    }

    public void SetDestination(Vector3 destination)
    {
        targetPos = destination;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= STUCK_DURATION && unstuckCooldown <= 0f)
            {
                // Move para posição aleatória próxima
                Vector2 offset = Random.insideUnitCircle.normalized * 0.1f;
                Vector3 safePosition = transform.position + new Vector3(offset.x, offset.y, 0f);
                SetDestination(safePosition);

                Debug.Log(name + " estava preso. Movido para: " + safePosition);

                unstuckCooldown = UNSTUCK_COOLDOWN_TIME;
                stuckTimer = 0f;
            }
        }
    }

}
