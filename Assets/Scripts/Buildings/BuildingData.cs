using UnityEngine;

public enum BuildingType
{
    TownCenter,
    Barracks,
    // outros tipos que quiseres
}

[CreateAssetMenu(fileName = "New Building Data", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public BuildingType buildingType;
    // Remove o GameObject actionPanelPrefab para evitar dependência de objetos da cena
}
