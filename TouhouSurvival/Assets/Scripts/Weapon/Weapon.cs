using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : MonoBehaviour
{
    public int id;
    public float damage;
    
    protected Unchord.Player _player;

    public virtual void Initialize(ItemData data)
    {
        name = "Weapon " + data.itemId;
        transform.parent = _player.transform;
        transform.localPosition = Vector3.zero;
        
        id = data.itemId;
        damage = data.baseDamage;
    }

    public virtual void LevelUp(float nextDamage, int nextCount, int nextPenetration)
    {
        damage = nextDamage;
    }

    protected virtual void Awake()
    {
        _player = GetComponentInParent<Unchord.Player>();
    }

    protected virtual void Start() { }

    protected virtual void Update() { }
}
