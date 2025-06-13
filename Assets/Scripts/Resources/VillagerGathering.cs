using UnityEngine;

public class VillagerGathering : MonoBehaviour
{
    public enum GatherState { Idle, MovingToResource, Gathering, Returning }
    public GatherState currentState = GatherState.Idle;

    private ResourceNode targetResource;
    private Transform townCenter;
    private int inventory = 0;
    private int maxInventory = 10;
    private UnitMovement movement;

    private float gatherTimer = 0f;
    private float gatherInterval = 1f;

    void Start()
    {
        movement = GetComponent<UnitMovement>();
        Debug.Log($"{name} está em estado: {currentState}");
    }

    void Update()
    {
        switch (currentState)
        {
            case GatherState.MovingToResource:
                if (ReachedDestination(targetResource.transform.position))
                {
                    Debug.Log($"{name} chegou ao recurso ({targetResource.name}), verificando disponibilidade...");
                    currentState = GatherState.Gathering;
                }
                break;

            case GatherState.Gathering:
                if (targetResource == null || targetResource.IsDepleted())
                {
                    Debug.Log($"{name}: recurso esgotado. À procura de outro...");
                    FindNewResource();
                    break;
                }

                gatherTimer += Time.deltaTime;
                if (gatherTimer >= gatherInterval)
                {
                    gatherTimer = 0f;
                    if (targetResource.Gather(1))
                    {
                        inventory++;
                        Debug.Log($"{name} recolheu 1 ({inventory}/{maxInventory})");

                        if (inventory >= maxInventory)
                        {
                            Debug.Log($"{name} inventário cheio. A ir ao Centro da Vila.");
                            currentState = GatherState.Returning;
                            movement.SetDestination(townCenter.position);
                        }
                    }
                }
                break;

            case GatherState.Returning:
                if (ReachedDestination(townCenter.position))
                {
                    Debug.Log($"{name} chegou ao Centro da Vila. A depositar...");
                    ResourceManager.Instance.AddResource(targetResource.resourceType, inventory);
                    Debug.Log($"{name} depositou {inventory} de {targetResource.resourceType}");
                    inventory = 0;
                    currentState = GatherState.MovingToResource;
                    movement.SetDestination(targetResource.transform.position);
                    Debug.Log($"{name} a voltar para o recurso ({targetResource.name})");
                }
                break;
        }
    }

    public void StartGathering(ResourceNode resource, Transform townCenter)
    {
        this.targetResource = resource;
        this.townCenter = townCenter;
        currentState = GatherState.MovingToResource;
        movement.SetDestination(resource.transform.position);
        Debug.Log($"{name} recebeu ordem para recolher: {resource.name} ({resource.resourceType})");
        Debug.Log($"{name} começou a recolher {resource.resourceType} de {resource.name}");
    }

    private bool ReachedDestination(Vector3 destination)
    {
        return Vector2.Distance(transform.position, destination) < 0.5f;
    }

    private void FindNewResource()
    {
        ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
        foreach (var node in nodes)
        {
            if (node.resourceType == targetResource.resourceType && !node.IsDepleted())
            {
                Debug.Log($"{name} encontrou novo recurso: {node.name}");
                targetResource = node;
                currentState = GatherState.MovingToResource;
                movement.SetDestination(node.transform.position);
                return;
            }
        }

        Debug.Log($"{name} não encontrou mais {targetResource.resourceType}. Ficando em Idle.");
        currentState = GatherState.Idle;
    }

}
