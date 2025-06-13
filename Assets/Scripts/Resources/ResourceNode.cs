using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType { Wood, Food, Stone, Gold }

    [Header("Configura��o do Recurso")]
    public ResourceType type = ResourceType.Wood;
    public int totalAmount = 100;

    // Propriedade p�blica para aceder ao tipo de recurso
    public ResourceType Type => type;

    // Propriedade p�blica para saber quanto ainda existe
    public int RemainingAmount => totalAmount;

    // M�todo que recolhe uma quantidade de recurso
    public void Collect(int amount)
    {
        totalAmount = Mathf.Max(totalAmount - amount, 0);

        if (totalAmount == 0)
        {
            // Pequeno delay antes de destruir (caso o alde�o ainda esteja por perto)
            Destroy(gameObject, 0.5f);
        }
    }
}
