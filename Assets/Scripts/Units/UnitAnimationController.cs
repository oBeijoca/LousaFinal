using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationController : MonoBehaviour
{
    private Animator anim;
    private Vector3 lastPosition;
    private string lastDirection = "Down";

    private enum State { Idle, Walk, Attack, Gather, Death }
    private State currentState = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        Debug.Log($"[AnimController] Inicializado com direção padrão: {lastDirection}");
    }

    void Update()
    {
        if (currentState != State.Idle && currentState != State.Walk)
            return; // ignora se estiver a atacar, recolher ou morrer

        UnitMovement movement = GetComponent<UnitMovement>();
        bool isMoving = movement != null && !movement.ReachedDestination();

        if (isMoving)
        {
            Vector3 delta = transform.position - lastPosition;
            Debug.Log("[AnimController] Movimento ativo: " + delta);
            SetDirection(delta);
            PlayWalk();
        }
        else
        {
            Debug.Log("[AnimController] Parado. A tocar idle.");
            PlayIdle();
        }

        lastPosition = transform.position;
    }

    public void SetDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            lastDirection = direction.x > 0 ? "Right" : "Left";
        }
        else
        {
            lastDirection = direction.y > 0 ? "Up" : "Down";
        }
        Debug.Log("[AnimController] Direção definida para: " + lastDirection);
    }

    public void PlayWalk()
    {
        if (currentState == State.Walk) return;
        currentState = State.Walk;
        string state = "Walk" + lastDirection;
        Debug.Log("[AnimController] A tocar animação: " + state);
        anim.Play(state, 0);
    }

    public void PlayIdle()
    {
        if (currentState == State.Idle) return;
        currentState = State.Idle;
        Debug.Log("[AnimController] A tocar animação: Idle");
        anim.Play("Idle", 0);
    }

    public void PlayAttack()
    {
        currentState = State.Attack;
        string state = "Atk" + lastDirection;
        Debug.Log("[AnimController] A tocar animação: " + state);
        anim.Play(state, 0);
        Invoke(nameof(ResetToIdle), 0.5f);
    }

    public void PlayDeath()
    {
        currentState = State.Death;
        Debug.Log("[AnimController] A tocar animação: Death");
        anim.Play("Death", 0);
    }

    public void PlayGatherAnimation(string resourceType)
    {
        currentState = State.Gather;
        string state = "";
        switch (resourceType)
        {
            case "Wood": state = "Chop"; break;
            case "Food": state = "Gather"; break;
            case "Gold": case "Stone": state = "Mine"; break;
            default:
                Debug.Log("[AnimController] Tipo de recurso desconhecido: " + resourceType);
                PlayIdle();
                return;
        }
        Debug.Log("[AnimController] A tocar animação de recolha: " + state);
        anim.Play(state);
    }

    public string GetLastDirection()
    {
        return lastDirection;
    }

    public void ResetToIdle()
    {
        if (currentState != State.Death)
        {
            currentState = State.Idle;
            PlayIdle();
        }
    }
}