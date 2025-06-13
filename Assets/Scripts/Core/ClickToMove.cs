using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private GameObject clickMarkerPrefab;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Debug.Log("ClickToMove iniciado. C�mara encontrada? " + (mainCamera != null));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Debug.Log("Bot�o direito clicado na posi��o do rato: " + mousePosition + " | Posi��o no mundo: " + worldPosition);

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
            Debug.Log("Colisores encontrados: " + hits.Length);

            foreach (var hitCollider in hits)
            {
                Debug.Log("Verificando colis�o com: " + hitCollider.name);
                ResourceNode resource = hitCollider.GetComponent<ResourceNode>();
                if (resource != null)
                {
                    Debug.Log("Clicou num recurso: " + resource.name);
                    foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
                    {
                        VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                        if (gatherer != null)
                        {
                            GameObject depositBuilding = GameObject.FindWithTag("Deposit");
                            Transform depositPoint = depositBuilding?.transform.Find("DepositPoint");
                            Transform targetDeposit = depositPoint != null ? depositPoint : depositBuilding?.transform;

                            if (targetDeposit != null)
                            {
                                Debug.Log("A iniciar recolha no ponto: " + targetDeposit.name);
                                gatherer.StartGathering(resource, targetDeposit);
                            }
                            else
                            {
                                Debug.LogWarning("Nenhum edif�cio de dep�sito v�lido encontrado!");
                            }
                        }
                    }

                    ShowMarker(worldPosition);
                    return;
                }
            }

            Debug.Log("Nenhum recurso detetado. A mover unidade para " + worldPosition);

            foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
            {
                VillagerGathering gatherer = unit.GetComponent<VillagerGathering>();
                if (gatherer != null) gatherer.StopGathering();

                UnitMovement movement = unit.GetComponent<UnitMovement>();
                if (movement != null)
                {
                    Debug.Log("A mover unidade: " + unit.name + " para " + worldPosition);
                    movement.SetDestination(worldPosition);
                }
            }

            ShowMarker(worldPosition);
        }
    }

    private void ShowMarker(Vector2 position)
    {
        if (clickMarkerPrefab != null)
        {
            GameObject existing = GameObject.FindWithTag("ClickMarker");
            if (existing != null) Destroy(existing);

            GameObject marker = Instantiate(clickMarkerPrefab, position, Quaternion.identity);
            marker.tag = "ClickMarker";
        }
    }
}
