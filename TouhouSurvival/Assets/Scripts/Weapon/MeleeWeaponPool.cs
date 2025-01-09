using UnityEngine;

public class MeleeWeaponPool : ObjectPoolBase
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
