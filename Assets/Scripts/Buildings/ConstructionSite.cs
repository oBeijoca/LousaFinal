using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public GameObject finalBuildingPrefab;
    public float buildTime = 5f;
    public Transform progressFill;

    private float currentProgress = 0f;
    private readonly HashSet<GameObject> builders = new();

    void Update()
    {
        if (builders.Count > 0)
        {
            currentProgress += Time.deltaTime * builders.Count;
            float t = Mathf.Clamp01(currentProgress / buildTime);
            if (progressFill != null)
                progressFill.localScale = new Vector3(t, 1, 1);

            if (currentProgress >= buildTime)
                FinishConstruction();
        }
    }

    void FinishConstruction()
    {
        Instantiate(finalBuildingPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void AddBuilder(GameObject builder)
    {
        builders.Add(builder);
    }

    public void RemoveBuilder(GameObject builder)
    {
        builders.Remove(builder);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out UnitMovement unit))
        {
            AddBuilder(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out UnitMovement unit))
        {
            RemoveBuilder(other.gameObject);
        }
    }
}
