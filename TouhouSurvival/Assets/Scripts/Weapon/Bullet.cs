using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int penetrations;

    private Player _player;
    private Rigidbody2D _rigidbody;
    
    private const int NotPenetrate = -1;

    public void Initialize(float damage, int penetrations = NotPenetrate, Vector3 direction = default)
    {
        this.damage = damage;
        this.penetrations = penetrations;

        if (penetrations > NotPenetrate)
        {
            _rigidbody.linearVelocity = direction * 15;
        }
    }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBulletOutOfArea(other))
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            return;
        }
        
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        if (penetrations == NotPenetrate)
        {
            return;
        }

        penetrations--;
        if (penetrations == NotPenetrate)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    bool IsBulletOutOfArea(Collider2D other)
    {
        return false;
    }
}
