using TMPro;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Vector2 target;
    private bool isMoving = false;

    public void SetDestination(Vector2 newTarget)
    {
        target = newTarget;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                isMoving = false;
            }
        }
    }

    public bool ReachedDestination()
    {
        return !isMoving;
    }

    public Vector2 GetTargetPosition()
    {
        return target;
    }

    public void SetTargetPosition(Vector3 newPos)
    {
        target = newPos;
        isMoving = true;
    }

}
