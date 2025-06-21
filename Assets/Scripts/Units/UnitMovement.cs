using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Vector2 target;
    private bool isMoving = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetTargetPosition(Vector3 newPos)
    {
        target = newPos;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 currentPos = rb.position;
            Vector2 direction = (target - currentPos).normalized;
            Vector2 newPosition = currentPos + direction * moveSpeed * Time.fixedDeltaTime;

            if (Vector2.Distance(currentPos, target) < 0.05f)
            {
                isMoving = false;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.MovePosition(newPosition);
            }
        }
    }

    public bool ReachedDestination() => !isMoving;
    public Vector2 GetTargetPosition() => target;
    public void ClearTarget() => Stop();
}
