using UnityEngine;
using UnityEngine.Serialization;

public class RangedWeapon : Weapon
{
    public int penetration;
    public float firingSpeed;

    private float _elapsedTime;
    
    public override void Initialize(ItemData data)
    {
        base.Initialize(data);

        penetration = data.basePenetration;
        firingSpeed = 0.3f;
    }

    public override void LevelUp(float nextDamage, int nextCount, int nextPenetration)
    {
        base.LevelUp(nextDamage, nextCount, nextPenetration);
        penetration += nextPenetration;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > firingSpeed)
        {
            _elapsedTime = 0.0f;
            Fire();
        }
    }

    private void Fire()
    {
        if (_player.Scanner.FindNearestTarget(out Transform nearestTarget))
        {
            // Transform bullet = GameManager.Instance.rangedWeaponPool.Pool.Get().transform;
            // bullet.position = transform.position;
            // bullet.parent = transform;
            //
            // Vector3 direction = (nearestTarget.position - transform.position).normalized;
            // bullet.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            //
            // bullet.GetComponent<Bullet>().Initialize(damage, penetration, direction);
        }
    }
}
