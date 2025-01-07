using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy[] prefabs;
    public ObjectPool<Enemy> Pool { get; private set; }

    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    private void Awake()
    {
        Pool = new ObjectPool<Enemy>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionCheck,
            defaultCapacity,
            maxSize);
    }

    private void ActionOnDestroy(Enemy obj)
    {
        Destroy(obj.gameObject);
    }

    private void ActionOnRelease(Enemy obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void ActionOnGet(Enemy obj)
    {
        obj.gameObject.SetActive(true);
    }

    private Enemy CreateFunc()
    {
        // PoolManager의 자식으로 생성
        Enemy enemy = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
        enemy.Pool = Pool;
        return enemy;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
