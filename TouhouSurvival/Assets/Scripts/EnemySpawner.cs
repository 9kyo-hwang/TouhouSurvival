using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private Transform[] _spawnPoints;
    private float _elapsedTime;

    private void Awake()
    {
        _spawnPoints = GetComponentsInChildren<Transform>();
        _elapsedTime = 0.0f;
    }

    void Start()
    {

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
        Enemy enemy = GameManager.Instance.enemyPool.Pool.Get();
        enemy.transform.position = _spawnPoints[Random.Range(1, _spawnPoints.Length)].position;
        // TODO: Stat Data Initialize
    }
}
