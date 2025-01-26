using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct EnemyStatData
{
    public float maxHp;
    public float attack;
    public float defense;
    public float movementSpeed;
    
    public static EnemyStatData operator +(EnemyStatData lhs, EnemyStatData rhs)
    {
        return new EnemyStatData
        {
            maxHp = lhs.maxHp + rhs.maxHp,
            attack = lhs.attack + rhs.attack,
            defense = lhs.defense + rhs.defense,
            movementSpeed = lhs.movementSpeed + rhs.movementSpeed
        };
    }
    
    public override string ToString()
    {
        return $"MaxHp: {maxHp}, Attack: {attack}, Defense: {defense}, MovementSpeed: {movementSpeed}";
    }
}

public class EnemyStatComponent : MonoBehaviour
{
    public float CurrentHp { get; private set; }
    public float Speed { get; private set; }

    [SerializeField] private EnemyStatData data;

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Initialize()
    {
        CurrentHp = data.maxHp;
        Speed = data.movementSpeed;
    }

    public float ApplyDamage(float damage)
    {
        float prevHp = CurrentHp;
        float actualDamage = Mathf.Clamp(damage, 0, damage);
        CurrentHp = Mathf.Clamp(prevHp - actualDamage, 0, data.maxHp);
        
        if (CurrentHp <= Mathf.Epsilon)
        {
            // Invoke Dead Event
            //OnHpZero?.Invoke();
        }
        
        return actualDamage;
    }
}
