using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationController : MonoBehaviour
{
    private Animator anim;
    private Vector3 lastPosition;
    private Direction lastDirection = Direction.Down;
    private string currentAnimation = "";

    private enum UnitState { Idle, Walk, Attack, Gather, Build, Death }
    private enum Direction { Up, Down, Left, Right }

    private UnitState currentState = UnitState.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (currentState == UnitState.Attack || currentState == UnitState.Gather || currentState == UnitState.Death || currentState == UnitState.Build)
            return;

        Vector3 delta = transform.position - lastPosition;
        bool isMoving = delta.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            SetDirection(delta);
            PlayWalk();
        }
        else
        {
            PlayIdle();
        }

        lastPosition = transform.position;
    }

    public void SetDirection(Vector2 direction)
    {
        Direction newDirection;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            newDirection = direction.x > 0 ? Direction.Right : Direction.Left;
        else
            newDirection = direction.y > 0 ? Direction.Up : Direction.Down;

        if (newDirection != lastDirection)
        {
            lastDirection = newDirection;
        }
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;
        currentAnimation = animName;
        anim.Play(animName);
    }

    public void PlayIdle()
    {
        if (currentState != UnitState.Idle)
        {
            currentState = UnitState.Idle;
            PlayAnimation("Idle");
        }
    }

    public void PlayWalk()
    {
        if (currentState != UnitState.Walk)
        {
            currentState = UnitState.Walk;
            PlayAnimation("Walk" + lastDirection);
        }
    }

    public void PlayAttack(Vector2? targetDirection = null)
    {
        if (targetDirection.HasValue)
        {
            SetDirection(targetDirection.Value);
        }

        currentState = UnitState.Attack;
        currentAnimation = ""; // força atualização
        PlayAnimation("Atk" + lastDirection);
        Invoke(nameof(ResetToIdle), 0.5f);
    }

    public void PlayDeath()
    {
        currentState = UnitState.Death;
        currentAnimation = ""; // força atualização
        PlayAnimation("Death");
    }

    public void PlayGatherAnimation(string resourceType)
    {
        currentState = UnitState.Gather;
        string animName = resourceType switch
        {
            "Wood" => "Chop",
            "Food" => "Gather",
            "Gold" => "Mine",
            "Stone" => "Mine",
            _ => "Idle"
        };

        PlayAnimation(animName);
    }

    public void PlayBuild()
    {
        currentState = UnitState.Build;
        PlayAnimation("Build");
    }

    public void ResetToIdle()
    {
        if (currentState != UnitState.Death)
        {
            currentState = UnitState.Idle;
            PlayIdle();
        }
    }

    public string GetCurrentDirection() => lastDirection.ToString();
    public string GetCurrentAnimation() => currentAnimation;
}
