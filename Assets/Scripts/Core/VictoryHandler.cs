using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryHandler : MonoBehaviour
{
    [SerializeField] private string victoryTargetName = "FINISH";

    void Start()
    {
        Debug.Log("[VictoryHandler] Inicializado.");
        InvokeRepeating(nameof(CheckForFinishTarget), 0f, 1f);
    }

    void CheckForFinishTarget()
    {
        Debug.Log("[VictoryHandler] A procurar por alvo...");

        var targets = FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (var h in targets)
        {
            Debug.Log($"[VictoryHandler] Encontrado: {h.gameObject.name}");

            if (h.gameObject.name.StartsWith(victoryTargetName))
            {
                h.OnDeath += HandleFinishDestroyed;
                Debug.Log($"[VictoryHandler] Registrado OnDeath para: {h.gameObject.name}");
                CancelInvoke(nameof(CheckForFinishTarget));
                break;
            }
        }
    }

    void HandleFinishDestroyed()
    {
        Debug.Log("🏆 Vitória! O centro inimigo foi destruído!");
        SceneManager.LoadScene("WinScene");
    }
}
