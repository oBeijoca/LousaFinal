using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType { Wood, Food, Stone, Gold }

    [Header("Configuração do Recurso")]
    public ResourceType type = ResourceType.Wood;
    public int totalAmount = 100;

    // Propriedade pública para aceder ao tipo de recurso
    public ResourceType Type => type;

    // Propriedade pública para saber quanto ainda existe
    public int RemainingAmount => totalAmount;

    // Método que recolhe uma quantidade de recurso
    public void Collect(int amount)
    {
        totalAmount = Mathf.Max(totalAmount - amount, 0);

        if (totalAmount == 0)
        {
            // Pequeno delay antes de destruir (caso o aldeão ainda esteja por perto)
            Destroy(gameObject, 0.5f);
        }
    }
}
