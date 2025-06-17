using UnityEngine;

[CreateAssetMenu(menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackRate = 1.0f;
    public float detectionRange = 3.0f;
    public float moveSpeed = 2.5f;
    public enum UnitType { Villager, Soldier, Archer }
    public UnitType unitType;
}
