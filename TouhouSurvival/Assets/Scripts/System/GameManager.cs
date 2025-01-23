using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public Player player;
    public EnemyPool enemyPool;
    public MeleeWeaponPool meleeWeaponPool;
    public RangedWeaponPool rangedWeaponPool;

    public int playerMaxLevel;
    [SerializeField] private PlayerStatData[] playerStatTable;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load Player Stat Data Table
        
        playerMaxLevel = playerStatTable.Length;
        Debug.Assert(playerMaxLevel > 0);
    }

    public PlayerStatData GetPlayerStat(int level)
    {
        return level < playerStatTable.Length ? playerStatTable[level] : new PlayerStatData();
    }
}
