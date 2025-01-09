using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemyPool : ObjectPoolBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void ActionOnDestroy(GameObject obj)
    {
        base.ActionOnDestroy(obj);
    }

    protected override void ActionOnRelease(GameObject obj)
    {
        base.ActionOnRelease(obj);
    }

    protected override void ActionOnGet(GameObject obj)
    {
        base.ActionOnGet(obj);
    }

    protected override GameObject CreateFunc()
    {
        return base.CreateFunc();
    }
}
