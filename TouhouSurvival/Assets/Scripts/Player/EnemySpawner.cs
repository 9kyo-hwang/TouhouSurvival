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
        GameObject enemy = GameManager.Instance.enemyPool.Pool.Get();
        enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
        
        // TODO: Stat Data Initialize
        enemy.GetComponent<Enemy>().Initialize(spawnData[/*level*/0]);
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