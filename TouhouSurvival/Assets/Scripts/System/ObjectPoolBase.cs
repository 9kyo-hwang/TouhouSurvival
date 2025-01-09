using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
public abstract class ObjectPoolBase : MonoBehaviour
{
    public ObjectPool<GameObject> Pool { get; private set; }
    
    [SerializeField] protected GameObject[] prefabs;
    [SerializeField] protected bool collectionCheck = true;
    [SerializeField] protected int defaultCapacity = 20;
    [SerializeField] protected int maxSize = 100;

    protected virtual void Awake()
    {
        Pool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionCheck,
            defaultCapacity, maxSize);
    }

    protected virtual void ActionOnDestroy(GameObject obj)
    {
        Destroy(obj.gameObject);
    }

    protected virtual void ActionOnRelease(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    protected virtual void ActionOnGet(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected virtual GameObject CreateFunc()
    {
        return Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
    }
}

