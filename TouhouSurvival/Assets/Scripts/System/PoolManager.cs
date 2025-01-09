using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PoolManager : MonoBehaviour
{
    public ObjectPool<GameObject>[] Pools { get; private set; }

    public GameObject[] prefabs;
    public bool collectionCheck;
    public int defaultCapacity;
    public int maxSize;

    private void Awake()
    {
        for (int index = 0; index < prefabs.Length; ++index)
        {
            Pools[index] = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
                collectionCheck, defaultCapacity, maxSize);
        }
    }
    
    private void ActionOnDestroy(GameObject obj)
    {
        Destroy(obj.gameObject);
    }

    private void ActionOnRelease(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void ActionOnGet(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }

    private GameObject CreateFunc()
    {
        return Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
    }
}
