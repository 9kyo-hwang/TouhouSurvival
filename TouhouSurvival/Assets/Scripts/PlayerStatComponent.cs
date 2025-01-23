using System;
using UnityEngine;

[Serializable]
public struct PlayerStatData
{
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
    [Header("Properties")] 
    public float CurrentHp { get; private set; }
    public float Speed { get; private set; }
    public float CurrentLevel { get; private set; }
    
    #region Delegates
    public event Action<float> OnHpChanged;
    public event Action OnHpZero;
    #endregion

    public PlayerStatData Total => _base + _modifier;
    private PlayerStatData _base;       // From Character Stat Table(In GameManager)
    private PlayerStatData _modifier;   // From Weapon Stat
    
    private void Start()
    {
        CurrentLevel = 1f;
        
        SetBaseStat((int)CurrentLevel);
        SetHp(_base.maxHp);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    public void SetBaseStat(int newLevel)
    {
        CurrentLevel = Mathf.Clamp(newLevel, 1, GameManager.Instance.playerMaxLevel);
        _base = GameManager.Instance.GetPlayerStat((int)CurrentLevel);
        Debug.Assert(_base.maxHp > 0);
    }

    public void SetModifierStat(PlayerStatData modifier)
    {
        _modifier = modifier;
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
        CurrentHp = Mathf.Clamp(newHp, 0, _base.maxHp);
        //OnHpChanged?.Invoke(Hp);
    }
}