using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType { Wood, Gold, Food, Stone }
    public ResourceType resourceType;
    public int amount = 10;

    public bool Gather(int quantity)
    {
        if (amount <= 0) return false;

        amount -= quantity;
        if (amount < 0) amount = 0;

        return true;
    }

    public bool IsDepleted() => amount <= 0;
}
