using UnityEngine;

public class VillagerGathering : MonoBehaviour
{
    public int inventorySize = 10;
    public float gatherInterval = 2f;
    public int gatherAmount = 1;

    private ResourceNode targetResource;
    private Transform townCenter;

    private int currentInventory = 0;
    private float gatherTimer = 0f;
    private UnitMovement movement;

    private enum State { Idle, MovingToResource, Gathering, Returning, Depositing }
    private State currentState = State.Idle;

    void Start()
    {
        movement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.MovingToResource:
                if (ReachedTarget(targetResource.transform.position))
                {
                    Debug.Log($"{name} chegou ao recurso ({targetResource.name}), verificando disponibilidade...");
                    currentState = State.Gathering;
                }
                break;

            case State.Gathering:
                if (targetResource == null || !targetResource.HasResources())
                {
                    Debug.Log($"{name} parou de recolher, recurso esgotado.");
                    currentState = State.Idle;
                    return;
                }

                gatherTimer += Time.deltaTime;
                if (gatherTimer >= gatherInterval)
                {
                    gatherTimer = 0f;
                    int gathered = targetResource.Gather(gatherAmount);
                    currentInventory += gathered;

                    Debug.Log($"{name} recolheu {gathered} ({currentInventory}/{inventorySize})");

                    if (currentInventory >= inventorySize)
                    {
                        Debug.Log($"{name} inventário cheio. A ir ao Centro da Vila.");
                        movement.SetDestination(townCenter.position);
                        currentState = State.Returning;
                    }
                }
                break;

            case State.Returning:
                if (ReachedTarget(townCenter.position))
                {
                    Debug.Log($"{name} chegou ao Centro da Vila. A depositar...");
                    currentState = State.Depositing;
                }
                break;

            case State.Depositing:
                Debug.Log($"{name} depositou {currentInventory} de {targetResource.type}");
                currentInventory = 0;

                if (targetResource != null && targetResource.HasResources())
                {
                    Debug.Log($"{name} a voltar para o recurso ({targetResource.name})");
                    gatherTimer = 0f; // reset para nova recolha
                    movement.SetDestination(targetResource.transform.position);
                    currentState = State.MovingToResource;
                }
                else
                {
                    Debug.Log($"{name} terminou porque o recurso acabou.");
                    targetResource = null;
                    currentState = State.Idle;
                }
                break;
        }

    }

    private bool ReachedTarget(Vector3 target)
    {
        return Vector2.Distance(transform.position, target) < 0.5f;
    }

    public void StartGathering(ResourceNode resource, Transform townCenterTransform)
    {
        targetResource = resource;
        townCenter = townCenterTransform;
        movement.SetDestination(resource.transform.position);
        currentState = State.MovingToResource;

        Debug.Log($"{name} recebeu ordem para recolher: {resource.name} ({resource.type})");

        Debug.Log($"{name} começou a recolher {resource.type} de {resource.name}");
    }
}
