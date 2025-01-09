using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage;

    protected virtual void Initialize() { }
    
    protected virtual void Awake() {}

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Update() { }
}
