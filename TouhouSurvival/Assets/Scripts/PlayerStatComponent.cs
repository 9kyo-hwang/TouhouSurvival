using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStatType
{
    Health,
    Attack,
    Defense,
    SKillRange,
    Speed,
    // etc
}

[Serializable]
public struct PlayerStatData
{
    public PlayerStatType statType;
    public float value;
    
    public float maxHp;
    public float attack;
    public float defense;
    public float movementSpeed;

    public static PlayerStatData operator +(PlayerStatData lhs, PlayerStatData rhs)
    {
        return new PlayerStatData
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

// StatComponent base를 두고 Player와 Enemy로 상속시켜...?
public class PlayerStatComponent : MonoBehaviour
{
    private Player _player;
    
    [Header("Properties")] 
    public float CurrentHp { get; private set; }
    public float Speed { get; private set; }
    public float CurrentLevel { get; private set; }
    
    #region Delegates
    public event Action<float> OnHpChanged;
    public event Action OnHpZero;
    #endregion
    
    public PlayerStatData Base { get; private set; }
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        CurrentLevel = 1f;
        Speed = 5f;
        
        SetBaseStat((int)CurrentLevel);
        SetHp(Base.maxHp);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    public void SetBaseStat(int newLevel)
    {
        CurrentLevel = Mathf.Clamp(newLevel, 1, _player.maxLevel);
        Base = _player.GetPlayerStat((int)CurrentLevel);
        //Debug.Assert(Base.maxHp > 0);
    }

    public float ApplyDamage(float damage)
    {
        float prevHp = CurrentHp;
        float actualDamage = Mathf.Clamp(damage, 0, damage);
        SetHp(prevHp - actualDamage);
        
        if (CurrentHp <= Mathf.Epsilon)
        {
            // Invoke Dead Event
            //OnHpZero?.Invoke();
        }
        
        return actualDamage;
    }

    private void SetHp(float newHp)
    {
        CurrentHp = Mathf.Clamp(newHp, 0, Base.maxHp);
        //OnHpChanged?.Invoke(Hp);
    }
}