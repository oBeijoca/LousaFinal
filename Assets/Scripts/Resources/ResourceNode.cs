using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType { Wood, Food, Gold, Stone }
    public ResourceType type;
    public int totalAmount = 50;

    public int Gather(int amount)
    {
        int gathered = Mathf.Min(amount, totalAmount);
        totalAmount -= gathered;
        return gathered;
    }

    public bool HasResources()
    {
        return totalAmount > 0;
    }
}
