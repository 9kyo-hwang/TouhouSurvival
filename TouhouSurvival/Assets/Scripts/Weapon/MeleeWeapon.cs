using System;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public int weaponCount;  // 배치할 근접 무기 개수
    public float rotationSpeed;

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        weaponCount += count;
        
        Batch();
    }

    protected override void Initialize()
    {
        base.Initialize();

        rotationSpeed = -150;
        Batch();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
    }

    private void Batch()
    {
        for (int index = 0; index < weaponCount; ++index)
        {
            Transform bullet;
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.meleeWeaponPool.Pool.Get().transform;
                bullet.parent = transform;
            }
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            
            bullet.Rotate(Vector3.forward * 360 * index / weaponCount);
            bullet.Translate(bullet.transform.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Initialize(damage);
        }
    }
}
