using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public SpawnData[] spawnData;  // data per level
    
    [SerializeField] private Transform[] spawnPoints;
    private float _elapsedTime;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        _elapsedTime = 0.0f;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        if (_elapsedTime >= 0.5f)
        {
            _elapsedTime = 0.0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        //GameObject enemy = GameManager.Instance.enemyPool.Pool.Get();
        //enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
        
        // Deprecated. 플레이어와 최대한 유사한 구조를 가지기 위해 각 몬스터마다 스탯 데이터를 들고 있도록 변경.
        // enemy.GetComponent<Enemy>().Initialize(spawnData[/*level*/0]);
    }
}

[Serializable]
public struct SpawnData
{
    public int enemyType;
    public float spawnTime;
    public int health;
    public float speed;
}