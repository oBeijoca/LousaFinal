using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecruitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject unitPrefab;
    public Transform spawnPoint;
    public int foodCost;
    public int woodCost;
    public int goldCost;
    public int stoneCost;
    public float trainingTime = 2f;

    public string unitName;
    public Image progressBar; // UI Image para a barra de progresso
    public TooltipUI tooltipUI; // TooltipUI específico deste botão

    private bool isTraining = false;

    void Start()
    {
        Debug.Log("[RecruitButton] Start chamado.");

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
            Debug.Log("[RecruitButton] Listener de clique adicionado.");
        }
        else
        {
            Debug.LogWarning("[RecruitButton] Nenhum componente Button encontrado.");
        }

        if (spawnPoint == null)
        {
            Building currentBuilding = BuildingUIManager.Instance?.CurrentBuilding;
            Transform parent = currentBuilding?.transform;
            if (parent != null)
            {
                foreach (Transform child in parent)
                    Debug.Log($"[RecruitButton] Filho encontrado: {child.name}");

                Transform found = parent.Find("SpawnPoint");
                if (found != null)
                {
                    spawnPoint = found;
                    Debug.Log($"[RecruitButton] SpawnPoint automaticamente definido: {spawnPoint.name}");
                }
                else
                {
                    Debug.LogWarning("[RecruitButton] Nenhum SpawnPoint encontrado como filho do edifício.");
                }
            }
        }

        if (progressBar != null)
            progressBar.fillAmount = 0f;
    }

    public void OnClick()
    {
        Debug.Log("[RecruitButton] Botão clicado.");
        if (isTraining)
        {
            Debug.LogWarning("[RecruitButton] Já está a treinar uma unidade.");
            return;
        }

        if (unitPrefab == null)
        {
            Debug.LogError("[RecruitButton] Prefab da unidade não definido!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[RecruitButton] SpawnPoint não definido!");
            return;
        }

        var rm = ResourceManager.Instance;
        bool hasResources =
            rm.GetResourceAmount(ResourceNode.ResourceType.Food) >= foodCost &&
            rm.GetResourceAmount(ResourceNode.ResourceType.Wood) >= woodCost &&
            rm.GetResourceAmount(ResourceNode.ResourceType.Gold) >= goldCost &&
            rm.GetResourceAmount(ResourceNode.ResourceType.Stone) >= stoneCost;

        if (!hasResources)
        {
            Debug.LogWarning("[RecruitButton] Recursos insuficientes para treinar " + unitName);
            return;
        }

        Debug.Log("[RecruitButton] Iniciando treino da unidade: " + unitName);
        StartCoroutine(TrainUnit());
    }

    IEnumerator TrainUnit()
    {
        isTraining = true;

        float elapsed = 0f;
        while (elapsed < trainingTime)
        {
            elapsed += Time.deltaTime;
            if (progressBar != null)
                progressBar.fillAmount = elapsed / trainingTime;
            yield return null;
        }

        ResourceManager.Instance.SpendResource(ResourceNode.ResourceType.Food, foodCost);
        ResourceManager.Instance.SpendResource(ResourceNode.ResourceType.Wood, woodCost);
        ResourceManager.Instance.SpendResource(ResourceNode.ResourceType.Gold, goldCost);
        ResourceManager.Instance.SpendResource(ResourceNode.ResourceType.Stone, stoneCost);

        GameObject unit = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        if (unit != null)
            Debug.Log("[RecruitButton] Unidade instanciada com sucesso: " + unit.name);
        else
            Debug.LogError("[RecruitButton] Falha ao instanciar a unidade!");

        if (progressBar != null)
            progressBar.fillAmount = 0f;

        isTraining = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI != null)
        {
            string tip = $"{unitName}\nComida: {foodCost}\nMadeira: {woodCost}\nOuro: {goldCost}\nPedra: {stoneCost}\nTempo: {trainingTime}s";
            tooltipUI.Show(tip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI?.Hide();
    }
}
