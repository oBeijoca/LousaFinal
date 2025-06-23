using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public GameObject finalBuildingPrefab;
    public float buildTime = 5f;
    public Transform progressFill;
    public AudioClip buildLoopSound;
    public AudioClip buildCompleteSound;

    private float currentProgress = 0f;
    private readonly HashSet<GameObject> builders = new();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Update()
    {
        if (builders.Count > 0)
        {
            currentProgress += Time.deltaTime * builders.Count;
            float t = Mathf.Clamp01(currentProgress / buildTime);
            if (progressFill != null)
                progressFill.localScale = new Vector3(t, 1, 1);

            if (!audioSource.isPlaying && buildLoopSound != null)
            {
                audioSource.clip = buildLoopSound;
                audioSource.Play();
            }

            if (currentProgress >= buildTime)
                FinishConstruction();
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void FinishConstruction()
    {
        if (buildCompleteSound != null)
            AudioSource.PlayClipAtPoint(buildCompleteSound, transform.position);

        GameObject building = Instantiate(finalBuildingPrefab, transform.position, Quaternion.identity);

        // Aumentar população se for cabana
        var buildingScript = building.GetComponent<Building>();
        if (buildingScript != null && buildingScript.buildingData != null)
        {
            if (buildingScript.buildingData.buildingType == BuildingType.Caban)
            {
                PopulationManager.Instance.IncreasePopulationCap(5);
                Debug.Log("[ConstructionSite] População máxima aumentada por construir uma cabana.");
            }
        }

        Destroy(gameObject);
    }


    public void AddBuilder(GameObject builder)
    {
        builders.Add(builder);
        var anim = builder.GetComponent<UnitAnimationController>();
        anim?.PlayBuild();
    }

    public void RemoveBuilder(GameObject builder)
    {
        builders.Remove(builder);
        var anim = builder.GetComponent<UnitAnimationController>();
        anim?.ResetToIdle();
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