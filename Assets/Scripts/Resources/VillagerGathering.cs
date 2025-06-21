using UnityEngine;

public class VillagerGathering : MonoBehaviour
{
    public enum State { Idle, MovingToResource, Gathering, Returning, Depositing }
    public State currentState = State.Idle;

    private ResourceNode currentResource;
    private Transform depositTarget;
    private UnitMovement movement;
    private ResourceNode.ResourceType resourceType;
    private int inventory = 0;
    private int inventoryCapacity = 10;
    private float gatherTimer = 0f;
    private float gatherInterval = 1f;
    private float depositRange = 0.6f;

    void Start()
    {
        movement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        if (currentState == State.Gathering)
        {
            if (currentResource == null || currentResource.RemainingAmount <= 0)
            {
                Debug.Log($"{name} recurso esgotado.");
                currentState = State.Idle;
                GetComponent<UnitAnimationController>()?.ResetToIdle();
                return;
            }

            if (Vector3.Distance(transform.position, currentResource.transform.position) > 0.2f)
                return;

            GetComponent<UnitAnimationController>()?.PlayGatherAnimation(resourceType.ToString());

            gatherTimer += Time.deltaTime;
            if (gatherTimer >= gatherInterval)
            {
                currentResource.Collect(1);
                inventory++;
                Debug.Log($"{name} recolheu 1 ({inventory}/{inventoryCapacity})");
                gatherTimer = 0f;

                if (inventory >= inventoryCapacity)
                {
                    Debug.Log($"{name} inventário cheio. A ir ao depósito.");
                    movement.SetTargetPosition(depositTarget.position);
                    currentState = State.Returning;
                    GetComponent<UnitAnimationController>()?.ResetToIdle();
                }
            }
        }
        else if (currentState == State.Returning)
        {
            if (Vector3.Distance(transform.position, depositTarget.position) < depositRange)
            {
                Debug.Log($"{name} chegou ao depósito.");
                currentState = State.Depositing;
                GetComponent<UnitAnimationController>()?.ResetToIdle();
            }
        }
        else if (currentState == State.Depositing)
        {
            ResourceManager.Instance.AddResource(resourceType, inventory);
            Debug.Log($"{name} depositou {inventory} de {resourceType}");
            inventory = 0;

            if (currentResource != null && currentResource.RemainingAmount > 0)
            {
                Debug.Log($"{name} a voltar para o recurso ({currentResource.name})");
                movement.SetTargetPosition(currentResource.transform.position);
                currentState = State.MovingToResource;
            }
            else
            {
                currentState = State.Idle;
                GetComponent<UnitAnimationController>()?.ResetToIdle();
            }
        }
    }

    public void StartGathering(ResourceNode resourceNode, Transform depositPoint)
    {
        currentResource = resourceNode;
        depositTarget = depositPoint;
        resourceType = resourceNode.Type;

        movement.SetTargetPosition(resourceNode.transform.position);
        currentState = State.MovingToResource;
        Debug.Log($"{name} recebeu ordem para recolher: {resourceNode.name} ({resourceType})");
    }

    public void StopGathering()
    {
        if (currentState == State.Gathering || currentState == State.MovingToResource)
        {
            Debug.Log($"{name} parou de recolher.");
            currentResource = null;
            currentState = State.Idle;
            GetComponent<UnitAnimationController>()?.ResetToIdle();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == State.MovingToResource && other.gameObject == currentResource?.gameObject)
        {
            Debug.Log($"{name} chegou ao recurso ({currentResource.name}), a recolher...");
            currentState = State.Gathering;
            gatherTimer = 0f;
            GetComponent<UnitAnimationController>()?.PlayGatherAnimation(resourceType.ToString());
        }
    }
}
