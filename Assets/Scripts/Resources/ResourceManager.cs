using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [System.Serializable]
    public class ResourceUI
    {
        public ResourceNode.ResourceType resourceType;
        public TextMeshProUGUI amountText;
        public int startingAmount = 0;
    }

    public List<ResourceUI> resourceUIList;

    private Dictionary<ResourceNode.ResourceType, int> resourceAmounts = new();
    private Dictionary<ResourceNode.ResourceType, TextMeshProUGUI> resourceTexts = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var entry in resourceUIList)
        {
            resourceAmounts[entry.resourceType] = entry.startingAmount;
            resourceTexts[entry.resourceType] = entry.amountText;
            UpdateResourceUI(entry.resourceType);
        }
    }

    public void AddResource(ResourceNode.ResourceType type, int amount)
    {
        if (!resourceAmounts.ContainsKey(type))
        {
            resourceAmounts[type] = 0;
        }

        resourceAmounts[type] += amount;
        UpdateResourceUI(type);
    }

    public bool SpendResource(ResourceNode.ResourceType type, int amount)
    {
        if (resourceAmounts.TryGetValue(type, out int current) && current >= amount)
        {
            resourceAmounts[type] -= amount;
            UpdateResourceUI(type);
            return true;
        }
        return false;
    }

    public int GetResourceAmount(ResourceNode.ResourceType type)
    {
        return resourceAmounts.TryGetValue(type, out int amount) ? amount : 0;
    }

    private void UpdateResourceUI(ResourceNode.ResourceType type)
    {
        if (resourceTexts.TryGetValue(type, out var text))
        {
            text.text = resourceAmounts[type].ToString();
        }
    }
}
